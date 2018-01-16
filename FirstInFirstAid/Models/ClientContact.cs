﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstInFirstAid.Models
{
    public class ClientContact
    {
        [Key]
        public int Id { get; set; }
        public string ContactName { get; set; }
        public string ContactPosition { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }

        public Client Client { get; set; }
        public ICollection<EventSegment> EventSegments { get; set; }

    }
}