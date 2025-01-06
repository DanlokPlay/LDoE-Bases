using Newtonsoft.Json;
using System.Collections.Generic;

public class Stack
{
    [JsonProperty("amount")]
    public int? Amount { get; set; }

    [JsonProperty("durability")]
    public int? Durability { get; set; }
}

public class Cells
{
    [JsonProperty("stack_id")]
    public string StackId { get; set; }

    [JsonProperty("stack")]
    public Stack Stack { get; set; }
}

public class Inventory
{
    [JsonProperty("cells")]
    public Dictionary<string, Cells> Cells { get; set; }
}

public class Inventories
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("inventory")]
    public Inventory Inventory { get; set; }
}

public class MotorcycleFuel
{
    public int Amount { get; set; }
    public int Boost { get; set; }
}

public class Resources
{
    [JsonProperty("motorcycle_fuel")]
    public MotorcycleFuel MotorcycleFuel { get; set; }

}

public class Slots
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("inventory")]
    public Inventory Inventory { get; set; }
}

public class Item
{
    [JsonProperty("unlocked")]
    public bool? Unlocked { get; set; }

    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("position")]
    public List<float> Position { get; set; }

    [JsonProperty("paint_pattern")]
    public string PaintPattern { get; set; }

    [JsonProperty("resources")]
    public Dictionary<string, MotorcycleFuel> Resources { get; set; }

    [JsonProperty("slots")]
    public Dictionary<string, Slots> Slots { get; set; }

    [JsonProperty("inventories")]
    public Dictionary<string, Inventories> Inventories { get; set; }
}

public class InputInventory
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("inventory")]
    public Inventory Inventory { get; set; }
}

public class Workbench
{
    [JsonProperty("input_inventories")]
    public Dictionary<string, InputInventory> InputInventories { get; set; }
}

public class Building
{
    [JsonProperty("x")]
    public int X { get; set; }

    [JsonProperty("y")]
    public int Y { get; set; }

    [JsonProperty("grade")]
    public int Grade { get; set; }

    [JsonProperty("rotation")]
    public int Rotation { get; set; }

    [JsonProperty("color_id")]
    public string ColorId { get; set; }

    [JsonProperty("unlocked")]
    public bool? Unlocked { get; set; }

    [JsonProperty("inventories")]
    public Dictionary<string, Inventories> Inventories { get; set; }

    [JsonProperty("workbench")]
    public Workbench Workbench { get; set; }
}

public class Items
{
    [JsonProperty("description_id")]
    public string DescriptionId { get; set; }

    [JsonProperty("item")]
    public Item Item { get; set; }

    [JsonProperty("building")]
    public Building Building { get; set; }
}

public class ResourceObjects
{
    [JsonProperty("items")]
    public Dictionary<string, Items> Items { get; set; }
}

public class Foundaments
{
    [JsonProperty("items")]
    public Dictionary<string, Items> Items { get; set; }
}

public class Furnitures
{
    [JsonProperty("items")]
    public Dictionary<string, Items> Items { get; set; }
}

public class Builder
{
    [JsonProperty("foundaments")]
    public Foundaments Foundaments { get; set; }

    [JsonProperty("walls")]
    public Walls Walls { get; set; }

    [JsonProperty("furnitures")]
    public Furnitures Furnitures { get; set; }
}

public class Walls
{
    [JsonProperty("items")]
    public Dictionary<string, Items> Items { get; set; }
}

public class LootObjects
{
    [JsonProperty("items")]
    public Dictionary<string, Items> Items { get; set; }
}

public class DragBoxObjects
{
    [JsonProperty("items")]
    public Dictionary<string, Items> Items { get; set; }
}

public class Transports
{
    [JsonProperty("items")]
    public Dictionary<string, Items> Items { get; set; }
}

public class Location
{
    [JsonProperty("resource_objects")]
    public ResourceObjects ResourceObjects { get; set; }

    [JsonProperty("loot_objects")]
    public LootObjects LootObjects { get; set; }

    [JsonProperty("builder")]
    public Builder Builder { get; set; }

    [JsonProperty("drag_box_objects")]
    public DragBoxObjects DragBoxObjects { get; set; }

    [JsonProperty("transports")]
    public Transports Transports { get; set; }
}

public class Raid
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("need_ban")]
    public bool NeedBan { get; set; }

    [JsonProperty("location")]
    public Location Location { get; set; }
}

public class RaidContainer
{
    [JsonProperty("raid")]
    public Raid Raid { get; set; }
}