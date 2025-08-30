using RimWorld;
using System.Collections.Generic;

namespace Outfitted
{
	/// <summary>
	/// Allows to use checks in code without causing referencing undefined StatDefs (if CE is not active).
	/// </summary>
	[DefOf]
	public static class StatDefOf_CE
	{
		// ceteam.combatextended
		[MayRequire("ceteam.combatextended")] public static StatDef WornBulk;
		[MayRequire("ceteam.combatextended")] public static StatDef ArmorRating_Electric;
		[MayRequire("ceteam.combatextended")] public static StatDef ElectricDamageMultiplier;
		[MayRequire("ceteam.combatextended")] public static StatDef Bulk;
		[MayRequire("ceteam.combatextended")] public static StatDef AmmoCaliber;
		[MayRequire("ceteam.combatextended")] public static StatDef LongRange;
		[MayRequire("ceteam.combatextended")] public static StatDef NightVisionEfficiency;
		[MayRequire("ceteam.combatextended")] public static StatDef NightVisionEfficiency_Implant;
		[MayRequire("ceteam.combatextended")] public static StatDef NightVisionEfficiency_Weapon;
		[MayRequire("ceteam.combatextended")] public static StatDef NightVisionEfficiency_Apparel;
		[MayRequire("ceteam.combatextended")] public static StatDef AimingAccuracy;
		[MayRequire("ceteam.combatextended")] public static StatDef ReloadSpeed;
		[MayRequire("ceteam.combatextended")] public static StatDef Suppressability;
		[MayRequire("ceteam.combatextended")] public static StatDef AverageSharpArmor;
		[MayRequire("ceteam.combatextended")] public static StatDef MeleeArmorPenetration;
		[MayRequire("ceteam.combatextended")] public static StatDef MeleeCritChance;
		[MayRequire("ceteam.combatextended")] public static StatDef MeleeParryChance;
		[MayRequire("ceteam.combatextended")] public static StatDef UnarmedDamage;
		[MayRequire("ceteam.combatextended")] public static StatDef BodyPartSharpArmor;
		[MayRequire("ceteam.combatextended")] public static StatDef BodyPartBluntArmor;
		[MayRequire("ceteam.combatextended")] public static StatDef SmokeSensitivity;
		[MayRequire("ceteam.combatextended")] public static StatDef CarryWeight;
		[MayRequire("ceteam.combatextended")] public static StatDef CarryBulk;
		[MayRequire("ceteam.combatextended")] public static StatDef StuffPower_Armor_Electric;
		[MayRequire("ceteam.combatextended")] public static StatDef MeleeDamage;
		[MayRequire("ceteam.combatextended")] public static StatDef StuffEffectMultiplierToughness;
		[MayRequire("ceteam.combatextended")] public static StatDef ToughnessRating;
		[MayRequire("ceteam.combatextended")] public static StatDef MeleePenetrationFactor;
		[MayRequire("ceteam.combatextended")] public static StatDef MeleeCounterParryBonus;
		[MayRequire("ceteam.combatextended")] public static StatDef SightsEfficiency;
		[MayRequire("ceteam.combatextended")] public static StatDef OneHandedness;
		[MayRequire("ceteam.combatextended")] public static StatDef UBGLInfo;
		[MayRequire("ceteam.combatextended")] public static StatDef ShotSpread;
		[MayRequire("ceteam.combatextended")] public static StatDef SwayFactor;
		[MayRequire("ceteam.combatextended")] public static StatDef MuzzleFlash;
		[MayRequire("ceteam.combatextended")] public static StatDef Caliber;
		[MayRequire("ceteam.combatextended")] public static StatDef BipodStats;
		[MayRequire("ceteam.combatextended")] public static StatDef TicksBetweenBurstShots;
		[MayRequire("ceteam.combatextended")] public static StatDef BurstShotCount;
		[MayRequire("ceteam.combatextended")] public static StatDef Recoil;
		[MayRequire("ceteam.combatextended")] public static StatDef MagazineCapacity;
		[MayRequire("ceteam.combatextended")] public static StatDef ReloadTime;
		[MayRequire("ceteam.combatextended")] public static StatDef ammoConsumedPerShotCount;
		[MayRequire("ceteam.combatextended")] public static StatDef AmmoGenPerMagOverride;
	}

	/// <summary>
	/// Original
	/// </summary>
	[DefOf]
	public static class StatDefOf_Rimworld
	{
		// ludeon.rimworld
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_RequiredPsylink;
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_CastingTime;
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_EntropyGain;
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_PsyfocusCost;
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_Range;
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_Duration;
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_EffectRadius;
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_GoodwillImpact;
		[MayRequire("ludeon.rimworld")] public static StatDef Ability_DetectChancePerEntropy;
		[MayRequire("ludeon.rimworld")] public static StatDef StuffEffectMultiplierArmor;
		[MayRequire("ludeon.rimworld")] public static StatDef StuffEffectMultiplierInsulation_Cold;
		[MayRequire("ludeon.rimworld")] public static StatDef StuffEffectMultiplierInsulation_Heat;
		[MayRequire("ludeon.rimworld")] public static StatDef ArmorRating_Sharp;
		[MayRequire("ludeon.rimworld")] public static StatDef ArmorRating_Blunt;
		[MayRequire("ludeon.rimworld")] public static StatDef ArmorRating_Heat;
		[MayRequire("ludeon.rimworld")] public static StatDef Insulation_Cold;
		[MayRequire("ludeon.rimworld")] public static StatDef Insulation_Heat;
		[MayRequire("ludeon.rimworld")] public static StatDef EnergyShieldEnergyMax;
		[MayRequire("ludeon.rimworld")] public static StatDef EnergyShieldRechargeRate;
		[MayRequire("ludeon.rimworld")] public static StatDef PackRadius;
		[MayRequire("ludeon.rimworld")] public static StatDef EquipDelay;
		[MayRequire("ludeon.rimworld")] public static StatDef MaxHitPoints;
		[MayRequire("ludeon.rimworld")] public static StatDef Mass;
		[MayRequire("ludeon.rimworld")] public static StatDef MarketValue;
		[MayRequire("ludeon.rimworld")] public static StatDef MarketValueIgnoreHp;
		[MayRequire("ludeon.rimworld")] public static StatDef SellPriceFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef RoyalFavorValue;
		[MayRequire("ludeon.rimworld")] public static StatDef Flammability;
		[MayRequire("ludeon.rimworld")] public static StatDef WorkToMake;
		[MayRequire("ludeon.rimworld")] public static StatDef DeteriorationRate;
		[MayRequire("ludeon.rimworld")] public static StatDef Beauty;
		[MayRequire("ludeon.rimworld")] public static StatDef BeautyOutdoors;
		[MayRequire("ludeon.rimworld")] public static StatDef Cleanliness;
		[MayRequire("ludeon.rimworld")] public static StatDef Comfort;
		[MayRequire("ludeon.rimworld")] public static StatDef Nutrition;
		[MayRequire("ludeon.rimworld")] public static StatDef FoodPoisonChanceFixedHuman;
		[MayRequire("ludeon.rimworld")] public static StatDef ShootingAccuracyTurret;
		[MayRequire("ludeon.rimworld")] public static StatDef CaravanBonusSpeedFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MedicalPotency;
		[MayRequire("ludeon.rimworld")] public static StatDef MedicalQualityMax;
		[MayRequire("ludeon.rimworld")] public static StatDef ConstructionSpeedFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MeditationFocusStrength;
		[MayRequire("ludeon.rimworld")] public static StatDef PsychicSensitivityOffset;
		[MayRequire("ludeon.rimworld")] public static StatDef PsychicSensitivityFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef PsychicEntropyMaxOffset;
		[MayRequire("ludeon.rimworld")] public static StatDef PsychicEntropyRecoveryRateOffset;
		[MayRequire("ludeon.rimworld")] public static StatDef FilthMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef CleaningTimeFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef WorkToBuild;
		[MayRequire("ludeon.rimworld")] public static StatDef DoorOpenSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef BedRestEffectiveness;
		[MayRequire("ludeon.rimworld")] public static StatDef TrapMeleeDamage;
		[MayRequire("ludeon.rimworld")] public static StatDef TrapSpringChance;
		[MayRequire("ludeon.rimworld")] public static StatDef ResearchSpeedFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MedicalTendQualityOffset;
		[MayRequire("ludeon.rimworld")] public static StatDef ImmunityGainSpeedFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef PowerPlantMaxPowerOuput;
		[MayRequire("ludeon.rimworld")] public static StatDef WorkTableWorkSpeedFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef WorkTableEfficiencyFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef JoyGainFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef SurgerySuccessChanceFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef RoomReadingBonus;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeDPS;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeDamageFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeCooldownFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeArmorPenetration;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeHitChance;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeDodgeChance;
		[MayRequire("ludeon.rimworld")] public static StatDef RangedCooldownFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef ShootingAccuracyPawn;
		[MayRequire("ludeon.rimworld")] public static StatDef ShootingAccuracyFactor_Touch;
		[MayRequire("ludeon.rimworld")] public static StatDef ShootingAccuracyFactor_Short;
		[MayRequire("ludeon.rimworld")] public static StatDef ShootingAccuracyFactor_Medium;
		[MayRequire("ludeon.rimworld")] public static StatDef ShootingAccuracyFactor_Long;
		[MayRequire("ludeon.rimworld")] public static StatDef AimingDelayFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MortarMissRadiusFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef PawnTrapSpringChance;
		[MayRequire("ludeon.rimworld")] public static StatDef IncomingDamageFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef StaggerDurationFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MoveSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef CrawlSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef MentalBreakThreshold;
		[MayRequire("ludeon.rimworld")] public static StatDef PsychicSensitivity;
		[MayRequire("ludeon.rimworld")] public static StatDef ToxicResistance;
		[MayRequire("ludeon.rimworld")] public static StatDef GlobalLearningFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef BedHungerRateFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef RestRateMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef EatingSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef ComfyTemperatureMin;
		[MayRequire("ludeon.rimworld")] public static StatDef ComfyTemperatureMax;
		[MayRequire("ludeon.rimworld")] public static StatDef ImmunityGainSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef InjuryHealingFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef CarryingCapacity;
		[MayRequire("ludeon.rimworld")] public static StatDef MeatAmount;
		[MayRequire("ludeon.rimworld")] public static StatDef LeatherAmount;
		[MayRequire("ludeon.rimworld")] public static StatDef MinimumHandlingSkill;
		[MayRequire("ludeon.rimworld")] public static StatDef PainShockThreshold;
		[MayRequire("ludeon.rimworld")] public static StatDef ForagedNutritionPerDay;
		[MayRequire("ludeon.rimworld")] public static StatDef FilthRate;
		[MayRequire("ludeon.rimworld")] public static StatDef AnimalsLearningFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef CaravanRidingSpeedFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MaxNutrition;
		[MayRequire("ludeon.rimworld")] public static StatDef LifespanFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MeditationFocusGain;
		[MayRequire("ludeon.rimworld")] public static StatDef PsychicEntropyMax;
		[MayRequire("ludeon.rimworld")] public static StatDef PsychicEntropyRecoveryRate;
		[MayRequire("ludeon.rimworld")] public static StatDef ToxicEnvironmentResistance;
		[MayRequire("ludeon.rimworld")] public static StatDef RestFallRateFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef EMPResistance;
		[MayRequire("ludeon.rimworld")] public static StatDef JoyFallRateFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef MaxFlightTime;
		[MayRequire("ludeon.rimworld")] public static StatDef FlightCooldown;
		[MayRequire("ludeon.rimworld")] public static StatDef Wildness;
		[MayRequire("ludeon.rimworld")] public static StatDef NegotiationAbility;
		[MayRequire("ludeon.rimworld")] public static StatDef PawnBeauty;
		[MayRequire("ludeon.rimworld")] public static StatDef ArrestSuccessChance;
		[MayRequire("ludeon.rimworld")] public static StatDef TradePriceImprovement;
		[MayRequire("ludeon.rimworld")] public static StatDef DrugSellPriceImprovement;
		[MayRequire("ludeon.rimworld")] public static StatDef SocialImpact;
		[MayRequire("ludeon.rimworld")] public static StatDef TameAnimalChance;
		[MayRequire("ludeon.rimworld")] public static StatDef TrainAnimalChance;
		[MayRequire("ludeon.rimworld")] public static StatDef BondAnimalChanceFactor;
		[MayRequire("ludeon.rimworld")] public static StatDef WorkSpeedGlobal;
		[MayRequire("ludeon.rimworld")] public static StatDef MiningSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef DeepDrillingSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef MiningYield;
		[MayRequire("ludeon.rimworld")] public static StatDef SmoothingSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef ResearchSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef AnimalGatherSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef AnimalGatherYield;
		[MayRequire("ludeon.rimworld")] public static StatDef PlantWorkSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef PlantHarvestYield;
		[MayRequire("ludeon.rimworld")] public static StatDef DrugHarvestYield;
		[MayRequire("ludeon.rimworld")] public static StatDef HuntingStealth;
		[MayRequire("ludeon.rimworld")] public static StatDef ConstructionSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef ConstructSuccessChance;
		[MayRequire("ludeon.rimworld")] public static StatDef FixBrokenDownBuildingSuccessChance;
		[MayRequire("ludeon.rimworld")] public static StatDef CleaningSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef ReadingSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef HackingSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef HackingStealth;
		[MayRequire("ludeon.rimworld")] public static StatDef MedicalTendSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef MedicalTendQuality;
		[MayRequire("ludeon.rimworld")] public static StatDef MedicalOperationSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef MedicalSurgerySuccessChance;
		[MayRequire("ludeon.rimworld")] public static StatDef SmeltingSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef GeneralLaborSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef DrugSynthesisSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef CookSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef FoodPoisonChance;
		[MayRequire("ludeon.rimworld")] public static StatDef DrugCookingSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef ButcheryFleshSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef ButcheryMechanoidSpeed;
		[MayRequire("ludeon.rimworld")] public static StatDef ButcheryFleshEfficiency;
		[MayRequire("ludeon.rimworld")] public static StatDef ButcheryMechanoidEfficiency;
		[MayRequire("ludeon.rimworld")] public static StatDef StuffPower_Armor_Sharp;
		[MayRequire("ludeon.rimworld")] public static StatDef StuffPower_Armor_Blunt;
		[MayRequire("ludeon.rimworld")] public static StatDef StuffPower_Armor_Heat;
		[MayRequire("ludeon.rimworld")] public static StatDef StuffPower_Insulation_Cold;
		[MayRequire("ludeon.rimworld")] public static StatDef StuffPower_Insulation_Heat;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeWeapon_AverageDPS;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeWeapon_AverageArmorPenetration;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeWeapon_DamageMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef MeleeWeapon_CooldownMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef SharpDamageMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef BluntDamageMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef AccuracyTouch;
		[MayRequire("ludeon.rimworld")] public static StatDef AccuracyShort;
		[MayRequire("ludeon.rimworld")] public static StatDef AccuracyMedium;
		[MayRequire("ludeon.rimworld")] public static StatDef AccuracyLong;
		[MayRequire("ludeon.rimworld")] public static StatDef RangedWeapon_Cooldown;
		[MayRequire("ludeon.rimworld")] public static StatDef RangedWeapon_DamageMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef RangedWeapon_ArmorPenetrationMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef RangedWeapon_RangeMultiplier;
		[MayRequire("ludeon.rimworld")] public static StatDef RangedWeapon_WarmupMultiplier;
		// ludeon.rimworld.royalty
		[MayRequire("ludeon.rimworld.royalty")] public static StatDef JumpRange;
		[MayRequire("ludeon.rimworld.royalty")] public static StatDef MeditationPlantGrowthOffset;
		[MayRequire("ludeon.rimworld.royalty")] public static StatDef PsychicEntropyGain;
		// ludeon.rimworld.ideology
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef SlaveSuppressionOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef TerrorSource;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef BiosculpterPodSpeedFactor;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef StyleDominance;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef Terror;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef ConversionPower;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef CertaintyLossFactor;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef SocialIdeoSpreadFrequencyFactor;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef ShootingAccuracyOutdoorsDarkOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef ShootingAccuracyOutdoorsLitOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef ShootingAccuracyIndoorsDarkOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef ShootingAccuracyIndoorsLitOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef MeleeHitChanceOutdoorsDarkOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef MeleeHitChanceOutdoorsLitOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef MeleeHitChanceIndoorsDarkOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef MeleeHitChanceIndoorsLitOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef MeleeDodgeChanceOutdoorsDarkOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef MeleeDodgeChanceOutdoorsLitOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef MeleeDodgeChanceIndoorsDarkOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef MeleeDodgeChanceIndoorsLitOffset;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef BiosculpterOccupantSpeed;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef SlaveSuppressionFallRate;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef AnimalProductsSellImprovement;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef SuppressionPower;
		[MayRequire("ludeon.rimworld.ideology")] public static StatDef PruningSpeed;
		// ludeon.rimworld.biotech
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef BabyPlayGainFactor;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef GeneticComplexityIncrease;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef AssemblySpeedFactor;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef BirthRitualQualityOffset;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MaxInstallCount;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MeleeDoorDamageFactor;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef ShootingAccuracyChildFactor;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef Fertility;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechBandwidth;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechControlGroups;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechRemoteRepairDistance;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechRemoteShieldDistance;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechRemoteShieldEnergy;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechFormingSpeed;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechRepairSpeed;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef SubcoreEncodingSpeed;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef BandwidthCost;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef ControlTakingTime;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechEnergyUsageFactor;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef WastepacksPerRecharge;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef MechEnergyLossPerHP;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef HemogenGainFactor;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef RawNutritionFactor;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef CancerRate;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef LearningRateFactor;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef GrowthVatOccupantSpeed;
		[MayRequire("ludeon.rimworld.biotech")] public static StatDef WorkSpeedGlobalOffsetMech;
		// ludeon.rimworld.anomaly
		[MayRequire("ludeon.rimworld.anomaly")] public static StatDef ContainmentStrength;
		[MayRequire("ludeon.rimworld.anomaly")] public static StatDef MinimumContainmentStrength;
		[MayRequire("ludeon.rimworld.anomaly")] public static StatDef ColdContainmentBonus;
		[MayRequire("ludeon.rimworld.anomaly")] public static StatDef EntityStudyRate;
		[MayRequire("ludeon.rimworld.anomaly")] public static StatDef StudyEfficiency;
		[MayRequire("ludeon.rimworld.anomaly")] public static StatDef ActivitySuppressionRate;
		[MayRequire("ludeon.rimworld.anomaly")] public static StatDef PsychicRitualQuality;
		[MayRequire("ludeon.rimworld.anomaly")] public static StatDef PsychicRitualQualityOffset;
		// ludeon.rimworld.odyssey
		[MayRequire("ludeon.rimworld.odyssey")] public static StatDef GravshipRange;
		[MayRequire("ludeon.rimworld.odyssey")] public static StatDef SubstructureSupport;
		[MayRequire("ludeon.rimworld.odyssey")] public static StatDef VacuumResistance;
		[MayRequire("ludeon.rimworld.odyssey")] public static StatDef FishingSpeed;
		[MayRequire("ludeon.rimworld.odyssey")] public static StatDef FishingYield;
		[MayRequire("ludeon.rimworld.odyssey")] public static StatDef PilotingAbility;
	}

	/// <summary>
	/// Add safe to dictionary. Checks for null-references.
	/// </summary>
	public static class StatDefOfHelper
	{
		public static void AddSafe(this IDictionary<StatDef, float> dict, StatDef def, float value)
		{
			if (dict == null || def == null) return;
			dict[def] = value;
		}
		public static Dictionary<StatDef, float> MakeDict(params (StatDef def, float val)[] items)
		{
			var dict = new Dictionary<StatDef, float>();
			foreach (var (def, val) in items)
				dict.AddSafe(def, val);
			return dict;
		}
	}
}
