Session session = await Session.CreateAsync(new User() { Character = new Character(), Id = "delegate" }, Util.GetPassword("openapi.apikey"));

string input = string.Empty;

do
{
	input = await Util.ReadLineAsync();

	if (input == "status")
	{
		session.User.Dump();
	}
	else
	{
		WeaponType? weapon = WeaponType.Options.FirstOrDefault(o => o.Value.Equals(input));
		
		if (weapon is not null)
		{
			await session.GetWeapon(weapon);
		}
		else 
		{
			await session.SendUserRequest(input);
		}
	}
}
while (input != "quit");

public class User
{
	public string Id { get; set; } = "discord id";
	public IReadOnlyList<ChatMessage> History { get; set; }
	public Character Character { get; set; }
	
	public User()
	{
		History = new List<ChatMessage>();
	}
}

public class Statistics
{
	public int QuestsCompleted { get; set; }
	public int Died { get; set; }
	public int PeopleKilled { get; set; }
	public int DarkChoices { get; set; }
	public int NeutralChoices { get; set; }
	public int LightChoices { get; set; }
	public int LocationsDiscovered { get; set; }
}

public class Character
{
	public string Prologue { get; set; }
	public Statistics Statistics { get; set; } = new();
	
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
	
	public string Class { get; set; } = "Sith Marauder";
	public List<string> ClassAbilities { get; set; } = new();
	public List<string> Feats { get; set; } = new();
	
	public long Credits { get; set; } = 500;
	public List<string> Items { get; set; } = new();	
	
	public Weapon Weapon { get; set; }
	public string Armor { get; set; } = "";
		
	public Location Location { get; set; } = new();

	public CharacterQuests Quests { get; set; }

	public Character()
	{
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
}

public class Location 
{
	public string Type { get; set; }
	public string Name { get; set; }	
	public Area Area { get; set; } = new();
}

public class Area
{
	public string Type { get; set; }
	public string Name { get; set; }
}

public class Weapon 
{
	public string Name { get; set; }
	public string Damage { get; set; }
	public string Description { get; set;}
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

public class Quest
{
	public string Name { get; set; }
	public string Description { get; set; }
	public int XP { get; set; }
	public int Credits { get; set; }
	public string[] Items { get; set; }
	public bool Completed { get; set; }
}

public class WeaponType
{	
	public static WeaponType MartialLightWeapon { get; } = new("Martial Lightweapon");
	public static WeaponType MartialVibroWeapon { get; } = new("Martial Vibroweapon");
	public static WeaponType SimpleBlaster = new("Simple Blaster");
	public static WeaponType SimpleLightWeapon { get; } = new("Simple Lightweapon");
	public static WeaponType SimpleVibroWeapon { get; } = new("Simple Vibroweapon");
	public static WeaponType ExoticBlaster { get; } = new("Exotic Blaster");
	public static WeaponType ExoticLightWeapon { get; } = new("Exotic Lightweapon");
	public static WeaponType ExoticVibroWeapon { get; } = new("Exotic Vibroweapon");
	
	public static IEnumerable<WeaponType> Options => 
			typeof(WeaponType)
				.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)				
				.Select(x => x.GetValue(null))
				.OfType<WeaponType>();

	public string Value { get; }

	private WeaponType(string value)
	{
		Value = value;
	}
}

public static class Templates
{	
	public static string GetWeapon(WeaponType type)
	{
		return 
"""
{
	"name": "[name of sw5e weapon]",
	"type": "[TYPE]",
	"cost": [cost of weapon as integer],
	"weight": [weight of weapon as integer],
	"damage": "[die damage e.g 3d8 energy]"
}
""".Replace("[TYPE]", type.Value);
	}
	
	public static string GetWithUserInput(string input)
	{
		return
"""
		{
			"narrative": "[INPUT]",
    		"delta": {
		        "xp": [0 or positive integer],
		        "credits": [0 or positive float],
		        "alignmentPoints": [negative or positive integer]",
		        "quests" [an array of quests that changed containing name of quest and completed field being true or false],
		        "location": {
		            "type": "[planet|space|space station]",
		            "name": "[name of given location type],		            
		            "area": {
		                "type": "[building|cave|underground|city|room|settlement]",
		                "name": "[name of the area type]"
		            }
		        }
		    }
		    "suggestions": [array of 3 suggestions. 1 neutral alignment, 1 dark alignment and 1 light alignment1]
		}
"""	.Replace("[INPUT]", input);
	}
	
	
}

public interface ISystemConversation
{
	public Conversation GetConversation();
}

public class WeaponSystem : ISystemConversation
{
	private readonly OpenAIAPI api;
	private readonly User user;

	public WeaponSystem(User user, OpenAIAPI api)
	{
		this.api = api;
		this.user = user;
	}

	public List<string> SystemMessages => new List<string>()
	{
		"You are a weapon generator using sw5e weapons",
		"Return the users message but replace all text identified with square brackets",		
		"Your response should be pure JSON with no additional information."
	};

	public Conversation GetConversation()
	{
		var conversation = api.Chat.CreateConversation(new ChatRequest { Model = Model.ChatGPTTurbo, user = user.Character.Name });
		conversation.RequestParameters.user = user.Character.Name;
		SystemMessages.ForEach(behaviour => conversation.AppendSystemMessage(behaviour));
		return conversation;		
	}
}

public class UserInputSystem : ISystemConversation
{
	private readonly OpenAIAPI api;
	private readonly User user;

	public List<string> SystemMessages => new List<string>()
	{
		"You are a Star Wars RPG Game Master.",
		"You allow expanded universe content.",
		"You are extremely flexible with user choices, but they cannot cheat.",
		"Replace the user input narrative with a higher quality output.",		
		"Replace all text identified with square brackets.",
		"Only return JSON.",
		"Do not chat in normal text"
	};

	public UserInputSystem(User user, OpenAIAPI api)
	{
		this.api = api;
		this.user = user;
	}
	
	public Conversation GetConversation()
	{
		var conversation = api.Chat.CreateConversation(new ChatRequest { Model = Model.ChatGPTTurbo, user = user.Character.Name });
		conversation.RequestParameters.user = user.Character.Name;
		SystemMessages.ForEach(behaviour => conversation.AppendSystemMessage(behaviour));
		
		var interactions = user.History.Where(x => x.Role == ChatMessageRole.Assistant || x.Role == ChatMessageRole.System).ToList();
		
		foreach(var chatMessage in interactions.TakeLast(interactions.Count / 2))
			conversation.AppendMessage(chatMessage);
			
		return conversation;
	}
}

public class Session
{
	private readonly OpenAIAPI api;
	private readonly User user;	
	private UserInputSystem userInputSystem;
	private WeaponSystem weaponSystem;

	private string CharacterJson => JsonSerializer.Serialize(user.Character, SerializeOptions);
	
	public User User => user;

	public static JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
	{
		WriteIndented = true,
		Converters = { new JsonStringEnumConverter() }
	};

	private Session(User user, string apiKey)
	{
		this.user = user;
		this.api = new OpenAIAPI(apiKey); // 
		this.api.Chat.DefaultChatRequestArgs.user = user.Character.Name;
		userInputSystem = new UserInputSystem(user, api);
		weaponSystem = new WeaponSystem(user, api);
	}

	public static async Task<Session> CreateAsync(User user, string apiKey)
	{
		var session = new Session(user, apiKey);
		
		if (user.Character.IsNew || string.IsNullOrWhiteSpace(user.Character.Prologue))
		{
			user.Character.Prologue = await session.CreatePrologue();	
		}
		
		return session;
	}
	
	public async Task<string> CreatePrologue()
	{
		var prompt =
		$"""
			Generate a short prologue for the character:
			
			{CharacterJson}			
		""";

		var completionResponse = await api.Completions.CreateCompletionAsync(new CompletionRequest { Prompt = prompt, MaxTokens = 500, Model = Model.DavinciText });
		
		return completionResponse.Completions[0].Text.Trim().Dump();		
	}
	
	public async Task GetWeapon(WeaponType type)
	{
		var conversation = weaponSystem.GetConversation();
		conversation.AppendUserInput(Templates.GetWeapon(type));
		var response = await conversation.GetResponseFromChatbotAsync();		
		response.Dump();
	}

	public async Task SendUserRequest(string userInput)
	{
		if (string.IsNullOrWhiteSpace(userInput))
			return;
		
		try
		{
			var conversation = userInputSystem.GetConversation();
			conversation.AppendUserInput(Templates.GetWithUserInput(userInput));
			
			var response = await conversation.GetResponseFromChatbotAsync();
			response.Dump();
			
			using (var jsonDocument = JsonDocument.Parse(response))
			{
				// We'll constantly store the state of N messages for context
				user.History = conversation.Messages;				
				
				var delta = jsonDocument.RootElement.GetProperty("delta");
				
				if (delta.TryGetProperty("xp", out var xpElement)  && xpElement.TryGetInt32(out var xpValue))
				{
					user.Character.XP += xpValue;
				}
				
				/*
					Call in to multiple sub chatgpt drivers on delta values such as:
					
					- WeaponSystem
					- QuestSystem
					
				
				*/
				

				if (delta.TryGetProperty("alignmentPoints", out var pointsElement) && pointsElement.TryGetInt32(out var pointsValue))
				{
					user.Character.AlignmentPoints += pointsValue;
				}

				if (delta.TryGetProperty("credits", out var credits) && credits.TryGetInt32(out var creditsValue))
				{
					user.Character.Credits += creditsValue;
				}
				
				if (delta.TryGetProperty("quests", out var quests))
				{
					foreach(var quest in quests.EnumerateArray())
					{
						var name = quest.GetProperty("name").GetString();
						var completed = quest.GetProperty("completed").GetBoolean();
						
						if (completed && user.Character.Quests.Active.Any(q => q.Name == name))
						{
							user.Character.Quests.Active.RemoveAll(x => x.Name == name);
							user.Character.Quests.Completed.Add(new Quest(){ Completed = true, Name = name });
						}
					}					
				}

				if (delta.TryGetProperty("location", out var locationElement))
				{					
					if (locationElement.TryGetProperty("type", out var locationTypeElement))
					{
						user.Character.Location.Type = locationTypeElement.GetString();
					}

					if (locationElement.TryGetProperty("name", out var locationNameElement))
					{
						user.Character.Location.Name = locationTypeElement.GetString();
					}

					if (locationElement.TryGetProperty("area", out var locationAreaElement))
					{
						if(locationAreaElement.TryGetProperty("type", out var areaTypeElement))
						{
							user.Character.Location.Area.Type = areaTypeElement.GetString();
						}

						if (locationAreaElement.TryGetProperty("name", out var areaNameElement))
						{
							user.Character.Location.Area.Type = areaNameElement.GetString();
						}
					}
				}
			}
		}
		catch (Exception exception)
		{			
			exception.Dump();
		}
	}
}
