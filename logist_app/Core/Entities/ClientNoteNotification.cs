using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logist_app.Core.Entities
{
    public class ClientNoteNotification
    {
        [System.Text.Json.Serialization.JsonPropertyName("client_id")]
        public int ClientId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("client_name")]
        public string ClientName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("notes_about_problems")]
        public string NotesAboutProblems { get; set; }
    }
}
