using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISoft.Metabase
{
    [Table("____column")]
    public class MBColumn
    {
        [Key, Column(Order = 0)]
        public Guid TableId { get; set; }

        [Key, Column(Order = 1)]
        [MaxLength(128)]
        public string Name { get; set; }
        
        [MaxLength(256)]
        public string Caption { get; set; }

        public int Ordinal { get; set; }

        public bool Nullable { get; set; }

        [Required]
        [MaxLength(64)]
        public string Type { get; set; }

        [MaxLength(128)]
        public string Spec { get; set; }

        public Nullable<long> CharMaxLength { get; set; }

        public Nullable<long> CharOctLength { get; set; }

        public Nullable<long> NumericPrecision { get; set; }

        public Nullable<long> NumericPrecisionRadix { get; set; }

        public Nullable<long> NumericScale { get; set; }

        public Nullable<long> DateTimePrecision { get; set; }

        [MaxLength(32)]
        public string CharsetSchema { get; set; }

        [MaxLength(32)]
        public string CharsetName { get; set; }

        [MaxLength(32)]
        public string CollationCatalog { get; set; }
    }
}
