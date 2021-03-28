﻿using System;
using Microsoft.ML.Data;

namespace HackAnalysis.Data
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        [ColumnName("comment")]
        public string comment { get; set; }
        public byte comment_positively { get; set; }
        public string sex { get; set; }
        public string date_birth { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}