﻿using System;

namespace abremir.AllMyBricks.Data.Models
{
    public class BricksetUserSet
    {
        public Set Set { get; set; }
        public bool Wanted { get; set; }
        public bool Owned { get; set; }
        public short QuantityOwned { get; set; }
        public DateTimeOffset LastChangeTimestamp { get; set; }
    }
}
