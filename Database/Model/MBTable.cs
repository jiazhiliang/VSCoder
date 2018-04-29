using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISoft.Metabase
{
    [Table("____table")]
    public class MBTable
    {
        [Key]
        public Guid TableId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Catalog { get; set; }

        [Required]
        [MaxLength(16)]
        public string Scheme { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Caption { get; set; }

        [Required]
        [MaxLength(16)]
        public string Type { get; set; }

        [MaxLength(128)]
        public string KeyInfo { get; set; }
    }
}
