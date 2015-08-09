using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace aWhere.Api.Weather.Sample {
    [DataContract]
    public class WeatherResponse {
        #region Constructor

        public WeatherResponse() {
            Conditions = new List<Conditions>();
        }

        #endregion Constructor

        #region Properties

        [DataMember(Name = "conditions", Order = 5, EmitDefaultValue = false)]
        [DisplayName("conditions")]
        public List<Conditions> Conditions { get; set; }

        [DataMember(Name = "dailyAttributes", Order = 4, EmitDefaultValue = false)]
        [DisplayName("dailyAttributes")]
        public Dictionary<string, double?> DailyAttributes { get; set; }

        [DataMember(Name = "date", Order = 3)]
        [DisplayName("date")]
        public DateTime Date { get; set; }

        [IgnoreDataMember]
        public bool IsConditionsRequest { get; set; }

        [DataMember(Name = "latitude", Order = 1)]
        [DisplayName("latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude", Order = 2)]
        [DisplayName("longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "requestId", Order = 0)]
        [DisplayName("requestId")]
        public int RequestId { get; set; }

        #endregion Properties

        #region Methods

        [OnSerializing]
        private void OnSerializing(StreamingContext context) {
            if (!IsConditionsRequest) {
                Conditions = null;
            } else {
                if (Conditions == null || Conditions.Count == 0) {
                    Conditions = new List<Conditions>();
                }
            }
        }

        #endregion Methods
    }
}