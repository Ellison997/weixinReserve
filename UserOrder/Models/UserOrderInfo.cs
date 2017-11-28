using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserOrder.Models
{
    public class UserOrderInfo
    {
        [Key]
        public int Id { get; set; }
        public string UName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH时mm分 }", ApplyFormatInEditMode = true)]
        public DateTime OrderDateTime { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH时mm分 }", ApplyFormatInEditMode = true)]
        public DateTime CancelDateTime { get; set; }
        public String State { get; set; }
        public int PeopleNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH时mm分 }", ApplyFormatInEditMode = true)]
        public DateTime UpdateDateTime { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }
    }
}