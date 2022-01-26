﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace DB.WeatherX.PwrTls.Models
{
    public partial class PointReal
    {
        public int Id { get; set; }
        public string SrcId { get; set; } = null!;
        public string SiteId { get; set; } = null!;
        public string MeasureId { get; set; } = null!;
        public double MeasureValue { get; set; }
        public DateTimeOffset MeasureTime { get; set; }
        public DateTimeOffset ForecastedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? Note64 { get; set; }
        public string? FreeNtext { get; set; }
    }
}