using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace JiraNotificationWebJob
{
    public partial class BaseModel
    {
        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("isLastPage")]
        public bool IsLastPage { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("values")]
        public Value[] Values { get; set; }
    }

    public partial class Links
    {
        [JsonProperty("self")]
        public Uri Self { get; set; }

        [JsonProperty("base")]
        public Uri Base { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("id")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("self")]
        public Uri Self { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("fields")]
        public Fields Fields { get; set; }
    }

    public partial class Fields
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("issuetype")]
        public Issuetype Issuetype { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("reporter")]
        public Reporter Reporter { get; set; }

        [JsonProperty("assignee")]
        public object Assignee { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }
    }

    public partial class Issuetype
    {
        [JsonProperty("self")]
        public Uri Self { get; set; }

        [JsonProperty("id")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("iconUrl")]
        public Uri IconUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("subtask")]
        public bool Subtask { get; set; }

        [JsonProperty("avatarId")]
        public long AvatarId { get; set; }
    }

    public partial class Reporter
    {
        [JsonProperty("self")]
        public Uri Self { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("avatarUrls")]
        public AvatarUrls AvatarUrls { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("accountType")]
        public string AccountType { get; set; }
    }

    public partial class AvatarUrls
    {
        [JsonProperty("48x48")]
        public Uri The48X48 { get; set; }

        [JsonProperty("24x24")]
        public Uri The24X24 { get; set; }

        [JsonProperty("16x16")]
        public Uri The16X16 { get; set; }

        [JsonProperty("32x32")]
        public Uri The32X32 { get; set; }
    }

    public partial class Status
    {
        [JsonProperty("self")]
        public Uri Self { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("iconUrl")]
        public Uri IconUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("statusCategory")]
        public StatusCategory StatusCategory { get; set; }
    }

    public partial class StatusCategory
    {
        [JsonProperty("self")]
        public Uri Self { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("colorName")]
        public string ColorName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
