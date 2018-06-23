using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class Plan
    {
        public int PlanId { get; set; }
        public int CreatorId { get; set; }
        public string Title { get; set; }
        public string Coordinator { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
        public DateTime created_at { get; set; }
        public List<RSVP> Participants { get; set; }
        public Plan()
        {
            Participants = new List<RSVP>();
        }
    }
}