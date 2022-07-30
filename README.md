# Tracery.Core
This is a rough port of [Tracery](https://github.com/galaxykate/tracery) by [Kate Compton](http://www.galaxykate.com/). The port is by Ishan Pranav and was heavily based on [Tracery.Net](https://github.com/josh-perry/Tracery.Net) by [Josh Parry](https://github.com/josh-perry). Both projects are available under the [Apache license 2.0](LICENSE.txt).

Tracery.Core targets .NET Standard, so it can be used in .NET 5+, .NET Core, .NET Framework, Unity, Xamarin, and compatible environments.
## Motivation
I re-implemented Josh Parry\'s Tracery.Net for use within [Rebus](https://github.com/ishanpranav/rebus), a multiplayer space trading game I was developing.

- Tracery.Net targets the .NET Framework 4.x and is not suitable for cross-platform development.
- It does not allow the Random number generator to be seeded. My project relied on determinism, so the ability to supply custom content selectors was essential.
- I was already using the new System.Text.Json APIs and wanted to avoid unnecessary dependencies on Newtonsoft.Json and YamlDotNet.

## Comparison
| Feature              |    Tracery.Core    | [Tracery.Net](https://github.com/josh-perry) |
| -------------------- | :----------------: | :------------------------------------------: |
| Target Framework     | .NET Standard 1.3+ | .NET Framework 4.5.2                         |
| Dependencies         |        None        | [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json), [YamlDotNet](https://github.com/aaubry/YamlDotNet) |
| Preferred serializer |  [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)  | [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) |
| Built-in modifiers   |         No         |                      Yes                     |
| Custom modifiers     |         Yes        |                      Yes                     |
| Custom selectors     |         Yes        |                      No                      |
| Deterministic        |       Optional     |                     Never                    |

## License
This repository is licensed with the [Apache license 2.0](LICENSE.txt).
## Usage
The [Grammar](Grammar.cs) class implements `IDictionary<string, IReadOnlyList<string>>`, so the `System.Text.Json.JsonSerializer` has built-in support for the type:

```csharp
await using (FileStream utf8Json = File.OpenRead("grammar.json"))
{
    Grammar grammar = await JsonSerializer.DeserializeAsync<Grammar>(utf8Json);
}
```

You can also initialize a grammar using either one of the [two C# dictionary initializer syntaxes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/how-to-initialize-a-dictionary-with-a-collection-initializer). For example:

```csharp
Grammar grammar = new()
{
    ["name"] = new[] { "Arjun", "Yuuma", "Darcy", "Mia", "Chiaki", "Izzi", "Azra", "Lina" },
    ["animal"] = new[] { "unicorn", "raven", "sparrow", "scorpion", "coyote", "eagle", "owl", "lizard", "zebra", "duck", "kitten" },
    ["mood"] = new[] { "vexed", "indignant", "impassioned", "wistful", "astute", "courteous" },
    ["story"] = new[] { "#hero# traveled with her pet #heroPet#.  #hero# was never #mood#, for the #heroPet# was always too #mood#." },
    ["origin"] = new[] { "#[hero:#name#][heroPet:#animal#]story#" }
};
```

Then use a content selector to generate a string:
```csharp
Random random = Random.Shared;
IContentSelector selector = new RandomContentSelector(random);

string result = grammar.Flatten(key: "#origin#", selector);
```
Kate Compton\'s example grammar above produces the following output:
```
Lina traveled with her pet duck. Lina was never indignant, for the duck was always too indignant.
Yuuma traveled with her pet unicorn. Yuuma was never wistful, for the unicorn was always too indignant.
Azra traveled with her pet coyote. Azra was never wistful, for the coyote was always too impassioned.
Yuuma traveled with her pet owl. Yuuma was never wistful, for the owl was always too courteous.
Azra traveled with her pet zebra. Azra was never impassioned, for the zebra was always too astute.
```
## Attribution
This software uses third-party libraries or other resources that may be
distributed under licenses different than the software. Please see the third-party notices included [here](THIRD-PARTY-NOTICES.txt).
