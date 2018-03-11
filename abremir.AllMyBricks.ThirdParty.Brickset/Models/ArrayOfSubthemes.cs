﻿using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlRoot(Namespace = Constants.BricksetApiNamespace), Description(Constants.MethodGetSubthemes)]
    public class ArrayOfSubthemes : List<Subthemes>
    {
    }
}