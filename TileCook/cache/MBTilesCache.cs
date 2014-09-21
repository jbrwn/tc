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
    
    public class MBTilesCache : ICache
    {

        private string _version;
        private bool _isCompressed;
        private string _database;
        private string _format;

        public MBTilesCache(string Database)
            : this(Database, null) { }

        public MBTilesCache(string Database, string Format)
        {
            //create mbtiles Database if not exists
            if (!File.Exists(Database))
            {
                CreateMBTiles(Database, Format);
            }
            this._database = Database;

            //cache version for internal use
            this._version = GetVersion();

            //set correct Format
            this._format = InitializeFormat(Format);

            //check for compressed schema
            this._isCompressed = isCompressed();
        }


        public string Database 
        {
            get { return this._database; } 
        }

        public byte[] Get(Coord coord, string Format)
        {
            int z = coord.Z;
            int x = coord.X;
            int y = coord.Y;

            byte[] img = null;
            if (Format.Equals(this._format, StringComparison.OrdinalIgnoreCase))
            {       
                using(SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.Database))))
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

        public void Put(Coord coord, string Format, byte[] image)
        {
            int z = coord.Z;
            int x = coord.X;
            int y = coord.Y;

            if (Format.Equals(this._format, StringComparison.OrdinalIgnoreCase))
            {
                if (this._isCompressed)
                {
                    string uuid = Guid.NewGuid().ToString();
                    using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.Database))))
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
                    using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.Database))))
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

        public void Delete(Coord coord, string Format)
        {
            int z = coord.Z;
            int x = coord.X;
            int y = coord.Y;

            if (Format.Equals(this._format, StringComparison.OrdinalIgnoreCase))
            {
                if (this._isCompressed)
                {
                    using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.Database))))
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
                    using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.Database))))
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

        private string InitializeFormat(string Format)
        {
            //get internal Format of MBTiles Database
            string internal_Format = GetInternalFormat();

            //begin logic for determining mbtiles Format
            if (Format == null && internal_Format == null)
            {
                string error = "Could not determine MBTiles Format.";
                if (this._version == "1.0.0")
                {
                    error = error + " Cannot determine Format of empty v1.0.0 Database.";
                }
                error = error + " Try explicitly configuring MBTiles cache Format.";
                throw new InvalidOperationException(error);
            }
            else if (Format == null && internal_Format != null)
            {
                return internal_Format;
            }
            else if (Format != null && internal_Format == null)
            {
                return Format;
            }
            else
            {
                if (!Format.Equals(internal_Format, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(string.Format("Format {0} does not match internal MBTiles Format {1}", Format, internal_Format));
                }
                return Format;
            }
        }

        private string GetInternalFormat()
        {
            string Format = null;
            using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.Database))))
            {
                con.Open();
                string query = "SELECT value FROM metadata WHERE name='Format'";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Format = dr.GetString(0);
                        }
                    }
                }
                //Format not required in MBTiles v1.0.0
                //TO DO: determine Format based on magic numbers (http://www.astro.keele.ac.uk/oldusers/rno/Computing/File_magic.html)
            }
            return Format;
        }

        private string GetVersion()
        {
            using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.Database))))
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

        private void CreateMBTiles(string Database, string Format)
        {
            if (Format == null)
            {
                throw new ArgumentNullException("Format must not be null");
            }
        }

        private bool isCompressed()
        {
            using (SQLiteConnection con = new SQLiteConnection((string.Format("Data Source={0}", this.Database))))
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
    }
}
