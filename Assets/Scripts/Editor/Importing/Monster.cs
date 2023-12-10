using Unity.Plastic.Newtonsoft.Json;

public class Monster
{
    public string name { get; set; }
    public string meta { get; set; }

    [JsonProperty("Armor Class")]
    public string ArmorClass { get; set; }

    [JsonProperty("Hit Points")]
    public string HitPoints { get; set; }
    public string Speed { get; set; }
    public string STR { get; set; }
    public string STR_mod { get; set; }
    public string DEX { get; set; }
    public string DEX_mod { get; set; }
    public string CON { get; set; }
    public string CON_mod { get; set; }
    public string INT { get; set; }
    public string INT_mod { get; set; }
    public string WIS { get; set; }
    public string WIS_mod { get; set; }
    public string CHA { get; set; }
    public string CHA_mod { get; set; }

    [JsonProperty("Saving Throws")]
    public string SavingThrows { get; set; }
    public string Skills { get; set; }
    public string Senses { get; set; }
    public string Languages { get; set; }
    public string Challenge { get; set; }
    public string Traits { get; set; }
    public string Actions { get; set; }

    [JsonProperty("Legendary Actions")]
    public string LegendaryActions { get; set; }
    public string img_url { get; set; }

    [JsonProperty("Damage Immunities")]
    public string DamageImmunities { get; set; }

    [JsonProperty("Condition Immunities")]
    public string ConditionImmunities { get; set; }

    [JsonProperty("Damage Resistances")]
    public string DamageResistances { get; set; }

    [JsonProperty("Damage Vulnerabilities")]
    public string DamageVulnerabilities { get; set; }
    public string Reactions { get; set; }
}

