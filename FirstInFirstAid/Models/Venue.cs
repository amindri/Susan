﻿using FirstInFirstAid.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Venue name is missing")]
        [CustomRemoteValidation("VenueExists", "Venues", AdditionalFields = "Id", ErrorMessage = "Venue Name already in use")]
        public String VenueName { get; set; }

        public Address Address { get; set; }

        public ICollection<EventSegment> EventSegments { get; set; }
    }
}