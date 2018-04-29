using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISoft.Metabase
{
    [Table("____property")]
    public class MBProperty
    {
        [Key, Column(Order = 0)]
        public Guid TableId { get; set; }

        [Key, Column(Order = 1)]
        [MaxLength(128)]
        public string Field { get; set; }

        [Key, Column(Order = 2)]
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(1024)]
        public string Value { get; set; }
    }
}
