using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace aWhere.Api.Weather.Sample {

    [DataContract]
    public class Conditions {

        [DataMember(Name = "startTime", Order = 1)]
        [DisplayName("startTime")]
        public DateTime StartTime { get; set; }

        [DataMember(Name = "endTime", Order = 2)]
        [DisplayName("endTime")]
        public DateTime EndTime { get; set; }

        [DataMember(Name = "condCode", Order = 3)]
        [DisplayName("condCode")]
        public string ConditionsCode { get; set; }

        [DataMember(Name = "condText", Order = 4)]
        [DisplayName("conditionText")]
        public string ConditionsText { get; set; }

        [DataMember(Name = "precipPercent", Order = 5)]
        [DisplayName("precipPercent")]
        public double? PrecipPercent { get; set; }

        [DataMember(Name = "sunPercent", Order = 6)]
        [DisplayName("sunPercent")]
        public double? SunPercent { get; set; }

        [DataMember(Name = "precip", Order = 7)]
        [DisplayName("precip")]
        public double? Precip { get; set; }

        [DataMember(Name = "cloudPercent", Order = 8)]
        [DisplayName("cloudPercent")]
        public double? CloudPercent { get; set; }

        [DataMember(Name = "wind", Order = 9)]
        [DisplayName("wind")]
        public double? Wind { get; set; }

        [DataMember(Name = "maxTemperature", Order = 10)]
        [DisplayName("maxTemperature")]
        public double? MaxTemperature { get; set; }

        [DataMember(Name = "minTemperature", Order = 11)]
        [DisplayName("minTemperature")]
        public double? MinTemperature { get; set; }

        [DataMember(Name = "solar", Order = 12)]
        [DisplayName("solar")]
        public double? Solar { get; set; }

        [DataMember(Name = "meanRelativeHumidity", Order = 13)]
        [DisplayName("meanRelativeHumidity")]
        public double? MeanRelativeHumidity { get; set; }
    }
}