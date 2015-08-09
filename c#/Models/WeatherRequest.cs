using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace aWhere.Api.Weather.Sample {
    [DataContract]
    public class WeatherRequest {
        #region Constructor

        public WeatherRequest() {
            ConditionsType = "standard";
            Intervals = 1;
            UtcOffset = new TimeSpan(0);
        }

        #endregion Constructor

        #region Properties

        [DataMember(Name = "attributes")]
        [DisplayName("attributes")]
        public List<string> Attributes { get; set; }

        [DataMember(Name = "baseTemp")]
        [DisplayName("baseTemp")]
        public double? BaseTemp { get; set; }

        [DataMember(Name = "conditionsType")]
        [DisplayName("conditionsType")]
        public string ConditionsType { get; set; }

        [DataMember(Name = "endDate")]
        [DisplayName("endDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [DataMember(Name = "gddMethod")]
        [DisplayName("gddMethod")]
        public string GddMethod { get; set; }

        [DataMember(Name = "intervals")]
        [DisplayName("intervals")]
        public short? Intervals { get; set; }

        [IgnoreDataMember]
        public bool IsConditionsRequest {
            get {
                if (Attributes == null || Attributes.Count == 0) {
                    return false;
                }

                return Attributes.Select(x => x.ToLower()).Contains("conditions");
            }
        }

        [DataMember(Name = "languageCode")]
        [DisplayName("languageCode")]
        public string LanguageCode { get; set; }

        [DataMember(Name = "latitude")]
        [DisplayName("latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        [DisplayName("longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "maxTempCap")]
        [DisplayName("maxTempCap")]
        public double? MaxTempCap { get; set; }

        [DataMember(Name = "minTempCap")]
        [DisplayName("minTempCap")]
        public double? MinTempCap { get; set; }

        [DataMember(Name = "plantingDate")]
        [DisplayName("plantingDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime? PlantingDate { get; set; }

        [DataMember(Name = "startDate")]
        [DisplayName("startDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataMember(Name = "units")]
        [DisplayName("units")]
        public string Units { get; set; }

        [DataMember(Name = "utcOffset")]
        [DisplayName("utcOffset")]
        public TimeSpan? UtcOffset { get; set; }

        #endregion Properties
    }
}