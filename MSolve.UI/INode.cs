﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSolve.UI
{
    public interface IGraphicalNode
    {
        double XCoordinate { get; set; }
        double YCoordinate { get; set; }
        double ZCoordinate { get; set; }
        
    }
}
