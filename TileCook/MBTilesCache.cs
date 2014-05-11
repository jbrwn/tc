using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Serialization;
using System.IO;

namespace TileCook
{
    [DataContract]
    class MBTilesCache : ICache
    {
        private MBTilesCache() { }

        private string _version;
        private bool _isCompressed;

        public MBTilesCache(string database)
            : this(database, null) { }

        
        public MBTilesCache(string database, string format)
        {
            //create mbtiles database if not exists
            if (!File.Exists(database))
            {
                CreateMBTiles(database, format);
            }
            this.database = database;

            //cache version for internal use
            this._version = GetVersion();

            //set correct format
            this.format = InitializeFormat(format);

            //check for compressed schema
            this._isCompressed = isCompressed();
        }

        [DataMember(IsRequired=true)]
        public string database { get; set; }

        [DataMember]
        public string format { get; set; }

        public byte[] get(int z, int x, int y, string format)
        {
            byte[] img = null;
            if (format.Equals(this.format, StringComparison.OrdinalIgnoreCase))
            {       
                using(SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.database))))
                {
                    con.Open();
                    string query = "SELECT tile_data FROM tiles WHERE zoom_level=@z AND tile_column=@x AND tile_row=@y";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@z", z);
                        cmd.Parameters.AddWithValue("@x", x);
                        cmd.Parameters.AddWithValue("@y", y);
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                img = (byte[])dr[0];
                            }
                        }
                    }
                }
            }
            return img;
        }

        public void put(int z, int x, int y, string format, byte[] image)
        {
            if (format.Equals(this.format, StringComparison.OrdinalIgnoreCase))
            {
                if (this._isCompressed)
                {
                    string uuid = Guid.NewGuid().ToString();
                    using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.database))))
                    {
                        con.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(con))
                        {
                            using (SQLiteTransaction tran = con.BeginTransaction())
                            {
                                //insert into map table
                                cmd.CommandText = "REPLACE INTO map (zoom_level, tile_column, tile_row, tile_id) VALUES (@z,@x,@y,@uuid)";
                                cmd.Parameters.AddWithValue("@z", z);
                                cmd.Parameters.AddWithValue("@x", x);
                                cmd.Parameters.AddWithValue("@y", y);
                                cmd.Parameters.AddWithValue("@uuid", uuid);
                                cmd.ExecuteNonQuery();

                                //clear paramaters
                                cmd.Parameters.Clear();

                                //insert into images table
                                cmd.CommandText = "REPLACE INTO images (tile_id, tile_data) VALUES (@uuid, @img)";
                                cmd.Parameters.AddWithValue("@uuid", uuid);
                                cmd.Parameters.AddWithValue("@img", image);
                                cmd.ExecuteNonQuery();

                                tran.Commit();
                            }
                        }
                    }
                }
                else
                {
                    using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.database))))
                    {
                        con.Open();
                        string query = "REPLACE INTO tiles (zoom_level, tile_column, tile_row, tile_data) VALUES (@z,@x,@y,@img)";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@z", z);
                            cmd.Parameters.AddWithValue("@x", x);
                            cmd.Parameters.AddWithValue("@y", y);
                            cmd.Parameters.AddWithValue("@img", image);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void delete(int z, int x, int y, string format)
        {
            if (format.Equals(this.format, StringComparison.OrdinalIgnoreCase))
            {
                if (this._isCompressed)
                {
                    using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.database))))
                    {
                        con.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(con))
                        {
                            using (SQLiteTransaction tran = con.BeginTransaction())
                            {
                                string uuid = null;

                                //get uuid
                                cmd.CommandText = "SELECT tile_id FROM map WHERE zoom_level=@z AND tile_column=@x AND tile_row=@y";
                                cmd.Parameters.AddWithValue("@z", z);
                                cmd.Parameters.AddWithValue("@x", x);
                                cmd.Parameters.AddWithValue("@y", y);

                                using (SQLiteDataReader dr = cmd.ExecuteReader())
                                {
                                    if (dr.Read())
                                    {
                                        uuid = dr.GetString(0);
                                    }
                                }

                                if (uuid != null)
                                {
                                    //Delete from map table 
                                    cmd.Parameters.Clear();
                                    cmd.CommandText = "DELETE FROM map WHERE zoom_level=@z AND tile_column=@x AND tile_row=@y";
                                    cmd.Parameters.AddWithValue("@z", z);
                                    cmd.Parameters.AddWithValue("@x", x);
                                    cmd.Parameters.AddWithValue("@y", y);
                                    cmd.ExecuteNonQuery();

                                    //Clean up images table?
                                    cmd.Parameters.Clear();
                                    cmd.CommandText = "SELECT 1 FROM map WHERE tile_id=@uuid";
                                    cmd.Parameters.AddWithValue("@uuid", uuid);
                                    var response = cmd.ExecuteScalar();
                                    if (response == null)
                                    {
                                        cmd.Parameters.Clear();
                                        cmd.CommandText = "DELETE FROM images WHERE tile_id=@uuid";
                                        cmd.Parameters.AddWithValue("@uuid", uuid);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                tran.Commit();
                            }
                        }
                    }
                }
                else
                {
                    using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.database))))
                    {
                        con.Open();
                        string query = "DELETE FROM tiles WHERE zoom_level=@z AND tile_column=@x AND tile_row=@y";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@z", z);
                            cmd.Parameters.AddWithValue("@x", x);
                            cmd.Parameters.AddWithValue("@y", y);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private string InitializeFormat(string format)
        {
            //get internal format of MBTiles database
            string internal_format = GetInternalFormat();

            //begin logic for determining mbtiles format
            if (format == null && internal_format == null)
            {
                string error = "Could not determine MBTiles FORMAT.";
                if (this._version == "1.0.0")
                {
                    error = error + " Cannot determine FORMAT of empty v1.0.0 database.";
                }
                error = error + " Try explicitly configuring MBTiles cache FORMAT.";
                throw new InvalidOperationException(error);
            }
            else if (format == null && internal_format != null)
            {
                return internal_format;
            }
            else if (format != null && internal_format == null)
            {
                return format;
            }
            else
            {
                if (!format.Equals(internal_format, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(string.Format("FORMAT {0} does not match internal MBTiles FORMAT {1}", format, internal_format));
                }
                return format;
            }
        }

        private string GetInternalFormat()
        {
            string format = null;
            using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.database))))
            {
                con.Open();
                string query = "SELECT value FROM metadata WHERE name='format'";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            format = dr.GetString(0);
                        }
                    }
                }
                //format not required in MBTiles v1.0.0
                //TO DO: determine format based on magic numbers (http://www.astro.keele.ac.uk/oldusers/rno/Computing/File_magic.html)
            }
            return format;
        }

        private string GetVersion()
        {
            using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.database))))
            {
                con.Open();
                string query = "SELECT value FROM metadata WHERE name='version'";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        string version = null;;
                        if (dr.Read())
                        {
                            version = dr.GetString(0);
                        }
                        return version;
                    }
                }
            }  
        }

        private void CreateMBTiles(string database, string format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("FORMAT must not be null");
            }
        }

        private bool isCompressed()
        {
            using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.database))))
            {
                con.Open();
                string query = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='tiles'";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    var result = cmd.ExecuteScalar();
                    return result == null ? true : false;
                }
            }  
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            //set absolution path if database path is relative
            if (!Path.IsPathRooted(this.database))
            {
                this.database = Path.Combine(LayerCache.ConfigDirectory, this.database);
            }

            //create mbtiles database if not exists
            if (!File.Exists(database))
            {
                CreateMBTiles(database, format);
            }
            this.database = database;

            //cache version for internal use
            this._version = GetVersion();

            //set format
            this.format = InitializeFormat(this.format);

            //check for compressed schema
            this._isCompressed = isCompressed();
        }
    }
}
