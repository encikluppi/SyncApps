using System;
using SyncApps.Models.ViewModels;

namespace SyncApps.Models.ViewModels
{
    public class PlatformVM
    {
        public int Id { get; set; }
        public string UniqueName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastUpdate { get; set; }
        public virtual ICollection<WellVM> Well { get; set; }
    }
}
