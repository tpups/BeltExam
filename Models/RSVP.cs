using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class RSVP
    {
        public int RSVPId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int PlanId { get; set; }
        public Plan Plan { get; set; }
    }
}