﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distancify.Migrations.Litium.LitiumGraphqlModel
{
    public class Channel
    {
        public string Id { get; set; }
        public ChannelFieldTemplate FieldTemplate { get; set; } 
    }
}
