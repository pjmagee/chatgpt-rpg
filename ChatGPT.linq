RPG rpg = await RPG.CreateInstance(new Character());

string input = string.Empty;

do
{
	var response = await rpg.SendRequest((input = await Util.ReadLineAsync()));	
	
	if (response is not null)
	{
		response.Dump();
	}
}
while (input != "quit");

public class Character
{
	public string Prologue { get; set; }
	public List<CharacterLog> StoryLog { get; set; }
	
	public bool IsNew { get; set; } = true;
	
	public string Name { get; set; } = "Delegate";
	public string Gender { get; set; } = "Male";	
	public string Species { get; set; } = "Human";
	
	public string Alignment { get; set; } = "Unlawful Dark side";	
	public long AlignmentPoints { get; set; } = 0;	
	
	public long Level { get; set; } = 1;
	public long XP { get; set; } = 0;
	
	public int Strength { get; set; } = 1;
	public int Dexterity { get; set; } = 1;	
	public int Constitution { get; set; } = 1;
	public int Intelligence { get; set; } = 1;
	public int Wisdom { get; set; } = 1;
	public int Charisma { get; set; } = 1;
	
	public string Class { get; set; } = "Berserker";
	public List<string> ClassAbilities { get; set; } = new();
	public List<string> Feats { get; set; } = new();
	
	public long Credits { get; set; } = 500;
	public List<string> Items { get; set; } = new();	
	
	public string Weapon { get; set; } = "";
	public string Armor { get; set; } = "";
	
	public string Planet { get; set; } = "Dantooine";
	public string Location { get; set; } = "Bacta Tank";

	public CharacterQuests Quests { get; set; }

	public Character()
	{
		StoryLog = new List<CharacterLog>();

		Quests = new();
		Quests.Active.Add(new Quest()
		{
			Completed = false,
			Credits = 1000,
			Name = "Find my starship",
			Description = "I must find my Starship so I can get back to my duties of being a Sith",
			XP = 500,
			Items = new[] { "medpack", "hunting knife", "datapad" }
		});
	}
	
	public void AppendLog(CharacterLog item)
	{
		if(StoryLog.Count >= 5)
			StoryLog.RemoveAt(0);
		
		StoryLog.Add(item);		
	}
}

public class CharacterQuests
{
	public List<Quest> Completed { get; set; }
	public List<Quest> Active { get; set; }

	public CharacterQuests()
	{
		Completed = new();
		Active = new();
	}
}

public class CharacterStoryPrompt
{
	public string Story { get; set; } = "";
	public List<CharacterAction> AvailableActions { get; set; }
}

public class CharacterAction
{
	public string DialogOption { get; set; } = "";
	public string ForceAlignment { get; set; } = "";
	public int AlignmentPoints { get; set; } = 0;
	public int XPGained { get; set; } = 0;
	public int CreditsGained { get; set; } = 0;
	public string NextLocation { get; set; } = "";
	public List<string> ItemsGained { get; set; } = new List<string>();
	
	public CharacterAction()
	{
		
	}
}

public class CharacterLog
{
	public DateTime EntryDate { get; set; }
	public string UserAction { get; set; }
	public string Narrative { get; set; }
}

public class Quest
{
	public string Name { get; set; }
	public string Description { get; set; }
	public int XP { get; set; }
	public int Credits { get; set; }
	public string[] Items { get; set; }
	public bool Completed { get; set; }
}

public static class JSONSchemas
{
	public static string ItemAttributesSchema = 
"""
{
    "$schema": "http://json-schema.org/draft-06/schema#",
    "$ref": "#/definitions/Response",
    "definitions": {
        "Response": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "name": {
                    "type": "string"
                },
                "description": {
                    "type": "string"
                },
                "type": {
                    "type": "string"
                },
                "cost": {
                    "type": "integer"
                },
                "weight": {
                    "type": "integer"
                },
                "damage": {
                    "type": "string"
                }
            },
            "required": [
                "cost",
                "damage",
                "description",
                "name",
                "type",
                "weight"
            ],
            "title": "Response"
        }
    }
}
""";


	public static string UserResponseSchema = 
"""
	{
    "$schema": "http://json-schema.org/draft-06/schema#",
    "$ref": "#/definitions/Response",
    "definitions": {
        "Response": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "narrative": {
                    "type": "string"
                },
                "gained": {
                    "$ref": "#/definitions/Gained"
                },
                "quests": {
                    "$ref": "#/definitions/Quests"
                },
                "location": {
                    "$ref": "#/definitions/Location"
                },
                "lost": {
                    "$ref": "#/definitions/Gained"
                },
                "suggestions": {
                    "$ref": "#/definitions/Suggestions"
                }
            },
            "required": [
                "gained",
                "location",
                "lost",
                "narrative",
                "quests",
                "suggestions"
            ],
            "title": "Response"
        },
        "Gained": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "xp": {
                    "type": "integer"
                },
                "credits": {
                    "type": "integer"
                },
                "alignmentPoints": {
                    "type": "integer"
                },
                "items": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "abilities": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "attributes": {
                    "$ref": "#/definitions/Attributes"
                }
            },
            "required": [
                "abilities",
                "alignmentPoints",
                "attributes",
                "credits",
                "items"
            ],
            "title": "Gained"
        },
        "Attributes": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "strength": {
                    "type": "integer"
                },
                "wisdom": {
                    "type": "integer"
                },
                "intelligence": {
                    "type": "integer"
                },
                "dexterity": {
                    "type": "integer"
                },
                "charisma": {
                    "type": "integer"
                }
            },
            "required": [
                "charisma",
                "dexterity",
                "intelligence",
                "strength",
                "wisdom"
            ],
            "title": "Attributes"
        },
        "Location": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "planet": {
                    "$ref": "#/definitions/Planet"
                },
                "building": {
                    "$ref": "#/definitions/Building"
                }
            },
            "required": [
                "building",
                "planet"
            ],
            "title": "Location"
        },
        "Building": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "name": {
                    "type": "string"
                },
                "room": {
                    "type": "string"
                }
            },
            "required": [
                "name",
                "room"
            ],
            "title": "Building"
        },
        "Planet": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
			    "name": {
                    "type": "string"
                },
                "latitude": {
                    "type": "integer"
                },
                "longitude": {
                    "type": "integer"
                }
            },
            "required": [
			    "name",
                "latitude",
                "longitude"
            ],
            "title": "Planet"
        },
        "Quests": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "accepted": {
                    "type": "array",
                    "items": {
                        "$ref": "#/definitions/Accepted"
                    }
                },
                "progressed": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "completed": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                }
            },
            "required": [
                "accepted",
                "completed",
                "progressed"
            ],
            "title": "Quests"
        },
        "Accepted": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "name": {
                    "type": "string"
                },
                "giver": {
                    "type": "string"
                },
                "rewards": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                }
            },
            "required": [
                "giver",
                "name",
                "rewards"
            ],
            "title": "Accepted"
        },
        "Suggestions": {
            "type": "object",
            "additionalProperties": false,
            "properties": {
                "choices": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                }
            },
            "required": [
                "choices"
            ],
            "title": "Suggestions"
        }
    }
}
""";

}

namespace QuickType
{
	using System;
	using System.Collections.Generic;

	using System.Text.Json;
	using System.Text.Json.Serialization;
	using System.Globalization;

	public class Response
	{
		[JsonPropertyName("gained")]
		public Gained Gained { get; set; }

		[JsonPropertyName("location")]
		public Location Location { get; set; }

		[JsonPropertyName("lost")]
		public Gained Lost { get; set; }

		[JsonPropertyName("narrative")]
		public string Narrative { get; set; }

		[JsonPropertyName("quests")]
		public Quests Quests { get; set; }

		[JsonPropertyName("suggestions")]
		public Suggestions Suggestions { get; set; }
	}

	public class Gained
	{
		[JsonPropertyName("abilities")]
		public List<string> Abilities { get; set; }

		[JsonPropertyName("alignmentPoints")]
		public long AlignmentPoints { get; set; }

		[JsonPropertyName("attributes")]
		public Attributes Attributes { get; set; }

		[JsonPropertyName("credits")]
		public long Credits { get; set; }

		[JsonPropertyName("items")]
		public List<string> Items { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("xp")]
		public long? Xp { get; set; }
	}

	public class Attributes
	{
		[JsonPropertyName("charisma")]
		public long Charisma { get; set; }

		[JsonPropertyName("dexterity")]
		public long Dexterity { get; set; }

		[JsonPropertyName("intelligence")]
		public long Intelligence { get; set; }

		[JsonPropertyName("strength")]
		public long Strength { get; set; }

		[JsonPropertyName("wisdom")]
		public long Wisdom { get; set; }
	}

	public class Location
	{
		[JsonPropertyName("building")]
		public Building Building { get; set; }

		[JsonPropertyName("planet")]
		public Planet Planet { get; set; }
	}

	public partial class Building
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("room")]
		public string Room { get; set; }
	}

	public partial class Planet
	{
		[JsonPropertyName("latitude")]
		public long Latitude { get; set; }

		[JsonPropertyName("longitude")]
		public long Longitude { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("name")]
		public string Name { get; set; }
	}

	public partial class Quests
	{
		[JsonPropertyName("accepted")]
		public List<Accepted> Accepted { get; set; }

		[JsonPropertyName("completed")]
		public List<string> Completed { get; set; }

		[JsonPropertyName("progressed")]
		public List<string> Progressed { get; set; }
	}

	public partial class Accepted
	{
		[JsonPropertyName("giver")]
		public string Giver { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("rewards")]
		public List<string> Rewards { get; set; }
	}

	public partial class Suggestions
	{
		[JsonPropertyName("choices")]
		public List<string> Choices { get; set; }
	}

	public static class Coordinate
	{
		public static Response FromJson(this string json) => JsonSerializer.Deserialize<Response>(json, Converter.Settings);
		public static string ToJson(this Response self) => JsonSerializer.Serialize(self, Converter.Settings);
	}

	internal static class Converter
	{
		public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
		{
			Converters =
			{
				new DateOnlyConverter(),
				new TimeOnlyConverter(),
				IsoDateTimeOffsetConverter.Singleton
			},
		};
	}

	public class DateOnlyConverter : JsonConverter<DateOnly>
	{
		private readonly string serializationFormat;
		public DateOnlyConverter() : this(null) { }

		public DateOnlyConverter(string? serializationFormat)
		{
			this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
		}

		public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			return DateOnly.Parse(value!);
		}

		public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString(serializationFormat));
	}

	public class TimeOnlyConverter : JsonConverter<TimeOnly>
	{
		private readonly string serializationFormat;

		public TimeOnlyConverter() : this(null) { }

		public TimeOnlyConverter(string? serializationFormat)
		{
			this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
		}

		public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			return TimeOnly.Parse(value!);
		}

		public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString(serializationFormat));
	}

	internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
	{
		public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

		private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

		private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
		private string? _dateTimeFormat;
		private CultureInfo? _culture;

		public DateTimeStyles DateTimeStyles
		{
			get => _dateTimeStyles;
			set => _dateTimeStyles = value;
		}

		public string? DateTimeFormat
		{
			get => _dateTimeFormat ?? string.Empty;
			set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
		}

		public CultureInfo Culture
		{
			get => _culture ?? CultureInfo.CurrentCulture;
			set => _culture = value;
		}

		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
		{
			string text;


			if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
				|| (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
			{
				value = value.ToUniversalTime();
			}

			text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

			writer.WriteStringValue(text);
		}

		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? dateText = reader.GetString();

			if (string.IsNullOrEmpty(dateText) == false)
			{
				if (!string.IsNullOrEmpty(_dateTimeFormat))
				{
					return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
				}
				else
				{
					return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
				}
			}
			else
			{
				return default(DateTimeOffset);
			}
		}


		public static readonly IsoDateTimeOffsetConverter Singleton = new IsoDateTimeOffsetConverter();
	}
}

public class RPG
{
	private readonly OpenAIAPI gameMaster;
	private readonly Character character;
	
	public Character Character => character;
	
	private string CharacterJson => JsonSerializer.Serialize(character, SerializeOptions);
	
	public static JsonSerializerOptions SerializeOptions = new JsonSerializerOptions 
	{ 
		WriteIndented = true, 
		Converters = { new JsonStringEnumConverter() } 
	};	

	public List<string> UserBehaviour => new List<string>()
	{
		$"""
		You only speak and respond in JSON
		""",
		$"""
		Schema:
		{JSONSchemas.UserResponseSchema}
		""",
		$"""
		You are the Game master for the character:
		{CharacterJson}
		"""
	};

	public List<string> WeaponBehaviour => new List<string>()
	{
		$"""
		You only speak and respond in JSON
		""",
		$"""
		Schema:
		{JSONSchemas.ItemAttributesSchema}
		""",		
		$"""
		You are the Game master for the character:
		{CharacterJson}
		"""
	};

	public Conversation CreateUserInputConversation()
	{
		var conversation = gameMaster.Chat.CreateConversation();
		conversation.RequestParameters.user = character.Name;		
		UserBehaviour.ForEach(behaviour => conversation.AppendSystemMessage(behaviour));
		return conversation;
	}
	
	public Conversation CreateItemAttributesConversation()
	{		
		Conversation conversation = gameMaster.Chat.CreateConversation();
		
		conversation.RequestParameters.user = character.Name;		
		UserBehaviour.ForEach(behaviour => conversation.AppendSystemMessage(behaviour));
		return conversation;
	}

	private RPG(Character character)
	{
		this.character = character;
		this.gameMaster = new OpenAIAPI(Util.GetPassword("openapi.apikey"));		
		this.gameMaster.Chat.DefaultChatRequestArgs.user = this.character.Name;		
	}

	public static async Task<RPG> CreateInstance(Character character)
	{
		var gameMaster = new RPG(character);	
		
		if (character.IsNew || string.IsNullOrWhiteSpace(character.Prologue))
		{
			await gameMaster.CreatePrologue();	
		}
		
		return gameMaster;
	}
	
	public async Task CreatePrologue()
	{
		var prompt =
		$"""
			Generate a short background prologue for the character {character.Name}.			
			It should take into consideration the characters profile provided in JSON:			
			{CharacterJson}			
		""";

		var completionResponse = await gameMaster.Completions.CreateCompletionAsync(new CompletionRequest { Prompt = prompt, MaxTokens = 500, Model = Model.DefaultModel });
		
		character.Prologue = completionResponse.Completions[0].Text;		
	}

	public async Task<Response?> SendRequest(string userAction)
	{
		if(string.IsNullOrWhiteSpace(userAction))
			return null;
		
		var conversation = CreateUserInputConversation();
		conversation.AppendUserInput(userAction);
		string chatResponse = await conversation.GetResponseFromChatbotAsync();
		Response response = null;
		
		try
		{
			response = Coordinate.FromJson(chatResponse);

			if (character.IsNew)
			{
				character.IsNew = false;
			}

			if (response.Narrative is not null)
			{
				character.AppendLog(new CharacterLog
				{
					EntryDate = DateTime.Now,
					UserAction = userAction,
					Narrative = response.Narrative,
				});

				character.XP += response.Gained.Xp.GetValueOrDefault();
				character.AlignmentPoints += response.Gained.AlignmentPoints;
				character.Credits += response.Gained.Credits;

				character.Planet = response.Location.Planet.Name;
				character.Location = $"{response.Location.Building.Name} {response.Location.Building.Room}";
			}

			return response;
		}
		catch (JsonException exception)
		{
			chatResponse.Dump("could not deserialize");
		}
		
		return null;
	}
}
