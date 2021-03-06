﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.Models
{
    public interface IMap<S, T> 
        where S: class 
        where T: class
    {
        S Map(T obj);
    }
}
