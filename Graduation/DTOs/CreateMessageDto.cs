﻿namespace Graduation.DTOs
{
    public class CreateMessageDto
    {
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
        //-------------------------------------
        public IFormFile File { get; set; }
    }
}
