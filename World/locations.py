from BaseClasses import Location
from . import items


class SableLocation(Location):
    game = "Sable"


def get_location_name_to_id():
    mask_locations_flat = {k: v for reg in masks.values() for k, v in reg.items()}
    id_to_name = (any_region_locations | ewer_locations | sansee_locations | redsee_locations | sodic_waste_locations | badlands_locations
                  | hakoa_locations | wash_locations | mask_locations_flat)
    table = {v: k for k, v in id_to_name.items()}
    # TODO: Add Badges & Key Items

    for i in range(1, 166):  # 4801-4965
        table[f"Chum {i}"] = 4800 + i

    assert len(list(map(lambda x: x, table.values()))) == len(set(table.values()))

    return table


masks = {
    "any": {
        4202: "Cartographer's Mask",
        4204: "Climbing Mask",
        4205: "Machinist's Mask",
        4206: "Entertainer's Mask",
        4207: "Angler Mask",
        4208: "Guard's Mask",
        4209: "Hercules Beetle Mask",
        4211: "Merchant's Mask",
        4213: "Scrapper Mask",
    },
    "ewer": {
        4210: "Ibexii Mask",
    },
    "sansee": {
        4203: "Chum Mask",
    },
    "redsee": {
        4201: "Whale Ship Mask",
        4214: "Shade of Eccria Mask",
    },
    "wash": {
        4212: "Sandwyrm Mask",
    },
}

any_region_locations = {
    # Random
    4701: "Fallow Pomegranate",
    4702: "Narrow Stalk Fig",
    4703: "Glowing Mushrooms",
    4704: "Hercules Beetle Larvae Husk",
    4705: "Larval Husk Package",
    4706: "Lightning Crystal",
    4707: "Melancholy Mushroom",
    4708: "Oasis Flower",
    4709: "Pink Neck Eggs",
    4710: "Prickly Pear",
    4711: "Scrap Metal",
    4712: "Shy Rijwur Pulp",
    4713: "Slicer Beetle Poo",
}
ewer_locations = {
    # Insects
    4732: "Yellow Elephant Beetle",
    4731: "Sunshine Butterfly",
    # Bike Parts
    4125: "Gliding Bike Booster",
    4126: "Gliding Bike Front",
    4127: "Gliding Bike Dye",
    # Palettes
    4418: "Ibexii Red Dye",
    # Clothes
    4277: "Ibexii Glider Top",
    4278: "Ibexii Glider Trousers",
    # Badges
    4023: "Machinist Badge (Ewer)",
}

sansee_locations = {
    4304: "Sansee Map",
    4972: "Hicaric Ring (Sansee)",
    4352: "Basic Wooden Rod",
    # Insects
    4723: "Dusk Firefly",
    # Fish
    4752: "Pill Fish",
    4744: "Pebble Eye",
    4743: "Dopey Dart",
    4742: "Bellow Guppy",
    # Bike Parts
    4101: "Angler Bike Engine",
    4102: "Angler Bike Front",
    4103: "Angler Bike Wings",
    4107: "Beetle Bike Booster",
    4108: "Beetle Bike Front",
    4109: "Beetle Bike Wings",
    4140: "Scrapper Bike Engine",
    4141: "Scrapper Bike Front",
    4142: "Scrapper Bike Wings",
    # Palettes
    4406: "Beetle Husk Dye",
    4427: "Salt Plain Pools Dye",
    4429: "Sandy Dye",
    # Clothes
    4261: "Chum Top",
    4262: "Chum Skirt",
    4263: "Bouldering Top",
    4269: "Machinist Top",
    4273: "Angler Top",
    4274: "Angler Bottoms",
    # Chum Queen Tears
    4966: "Chum Queen Tear 1",
    4967: "Chum Queen Tear 2",
    4968: "Chum Queen Tear 3",
    4969: "Chum Queen Tear 4",
    4970: "Chum Queen Tear 5",
    4971: "Chum Queen Tear 6",
    # Badges
    4001: "Angler Badge (Sansee) 1",
    4002: "Angler Badge (Sansee) 2",
    4003: "Angler Badge (Sansee) 3",
    4004: "Beetle Badge (Sansee)",
    4007: "Cartographer's Badge (Sansee)",
    4024: "Machinist Badge (Sansee)",
    4030: "Scrapper's Badge (Sansee) (30 Scrap)",
    4031: "Scrapper's Badge (Sansee) (60 Scrap)",
    4032: "Scrapper's Badge (Sansee) (90 Scrap)",

}

redsee_locations = {
    4306: "Redsee Map",
    4973: "Hicaric Ring (Redsee)",
    4356: "Atomic Heart Keycard",
    4357: "Broken Atomic Heart Powercore",
    4358: "Repaired Atomic Heart Powercore",
    4714: "Magpie Key",
    # Insects
    4726: "Oasis Dragonfly",
    4729: "Shaded Leaf Butterfly",
    4730: "Stepwell Dragonfly",
    # Fish
    4749: "Sucker Fish",
    4753: "Storm Fish",
    4756: "Sand Ray",
    4748: "Hummer",
    # Atomic Heart Clues
    4761: "CLUE: Promissory Note",
    4762: "CLUE: Empty Socket",
    4763: "CLUE: Large Feathers",
    4764: "CLUE: Smashed Glass",
    4765: "SUSPECT: Climber Garay",
    4766: "SUSPECT: Machinist Hamza",
    4767: "SUSPECT: Merchant Iria",
    # Bike Parts
    4104: "Whale Ship Bike Engine",
    4105: "Whale Ship Bike Front",
    4106: "Whale Ship Bike Wing",
    4111: "Cartographer Bike Front",
    4113: "Delivery Bike Engine",
    4114: "Delivery Bike Front",
    4115: "Delivery Bike Wings",
    4122: "Giraffe Bike Booster",
    4123: "Giraffe Bike Front",
    4124: "Giraffe Bike Wings",
    4137: "Salt Bike Engine",
    4138: "Salt Bike Front",
    4139: "Salt Bike Wings",
    4143: "Shade Bike Booster",
    4144: "Shade Bike Front",
    4145: "Shade Bike Wings",
    4146: "Speedster Bike Engine",
    4147: "Speedster Bike Front",
    4148: "Speedster Bike Wing",
    # Palettes
    4401: "Atomic Core Dye",
    4402: "Atomic Shell Dye",
    4411: "Dusty Monument Dye",
    4412: "Eccrine Green Dye",
    4435: "Wind Chime Dye",
    # Clothes
    4252: "Atomic Priesthood Trousers",
    4253: "Whale Ship Top",
    4254: "Whale Ship Trousers",
    4255: "Beetle Station Top",
    4256: "Beetle Station Trousers",
    4257: "Biker Top",
    4258: "Bikers Trousers",
    4267: "Eccrine Top",
    4268: "Eccrine Trousers",
    4275: "Eccrine Guard Top",
    4276: "Eccrine Guard Trousers",
    4279: "Monumental Stone Top",
    4281: "Scrapper Top",
    4282: "Scrapper Bottoms",
    4283: "Shade Top",
    4284: "Shade Trousers",
    4285: "Sand Surfer Top",
    4286: "Sand Surfer Shorts",
    # Badges
    4010: "Cartographer's Badge (Redsee)",
    4014: "Climbing Badge (Redsee)",
    4016: "Entertainer's Badge (Redsee) 1",
    4017: "Entertainer's Badge (Redsee) 2",
    4021: "Guard's Badge (Redsee)",
    4027: "Merchant Badge (Redsee) 1",
    4028: "Merchant Badge (Redsee) 2",
    4029: "Merchant Badge (Redsee) 3",
}

badlands_locations = {
    4301: "Badlands Map",
    4975: "Hicaric Ring (Badlands)",
    # Fish
    4755: "Light Koi",
    4741: "Scabby Fish",
    4746: "Flutter Thrust",
    # Bike Parts
    4119: "Eyries Bike Booster",
    4120: "Eyries Bike Front",
    4121: "Eyries Bike Wings",
    4131: "Hicaric Ring Bike Booster",
    4132: "Hicaric Ring Bike Front",
    4133: "Hicaric Ring Bike Wings",
    # Palettes
    4433: "Sunsets Dye",
    # Clothes
    4264: "Bouldering Trousers",
    4271: "Eyries Top",
    4272: "Eyries Trousers",
    4280: "Monumental Stone Bottoms",
    # Badges
    4012: "Cartographer's Badge (Badlands)",
    4022: "Guard's Badge (Badlands)",
    4013: "Climbing Badge (Badlands)",
}

wash_locations = {
    4307: "The Wash Map",
    4977: "Hicaric Ring (The Wash)",
    4353: "Trickster Atomic Rod",
    # Insects
    4721: "Chalk Butterfly",
    4725: "Nimoor Butterflies",
    4727: "Orange Winged Beetles",
    # Fish
    4745: "Bloated Eye Clam",
    4751: "Little Kicker",
    4750: "Trifle Jelly",
    4757: "Ghost Soul",
    # Fish Clues
    4771: "Scabby Fish Clue",
    4772: "Bellow Guppy Clue",
    4773: "Dopey Dart Clue",
    4774: "Pebble Eye Clue",
    4775: "Bloated Eye Clam Clue",
    4776: "Flutter Thrust Clue",
    4777: "Shelled Cup Clue",
    4778: "Hummer Clue",
    4779: "Sucker Fish Clue",
    4780: "Trifle Jelly Clue",
    4781: "Little Kicker Clue",
    4782: "Pill Fish Clue",
    4783: "Storm Fish Clue",
    4784: "Teacup Angler Clue",
    4785: "Light Koi Clue",
    4786: "Sand Ray Clue",
    4787: "Ghost Soul Clue",
    # Bike Parts
    4112: "Cartographer Bike Wings",
    4116: "Dragonfly Bike Engine",
    4117: "Dragonfly Bike Front",
    4118: "Dragonfly Bike Wings",
    4134: "Nomadic Bike Engine",
    4135: "Nomadic Bike Front",
    4136: "Nomadic Bike Wing",
    # Palettes
    4404: "Azure Dye",
    4419: "Lavender Flash Dye",
    4425: "Pyrausta Grey Dye",
    4434: "Wash Blue Dye",
    # Clothes
    4251: "Atomic Priesthood Torso",
    4265: "Butterfly Top",
    4266: "Butterfly Trousers",
    4270: "Machinist Trousers",
    4287: "Wash Top",
    4288: "Wash Trousers",
    # Badges
    4005: "Beetle Badge (The Wash)",
    4008: "Cartographer's Badge (The Wash)",
    4019: "Guard's Badge (The Wash)",
    4025: "Machinist Badge (The Wash)",
}

hakoa_locations = {
    4302: "Hakoa Map",
    4976: "Hicaric Ring (Hakoa)",
    # Insects
    4722: "Crystal Butterfly",
    4724: "Hakoan Glowworms",
    # Fish
    4754: "Teacup Angler",
    # Bike Parts
    4110: "Cartographer Bike Engine",
    4128: "Lightning Crystal Bike Booster",
    4129: "Lightning Crystal Bike Front",
    4130: "Lightning Crystal Bike Wings",
    # Palettes
    4417: "Hakoan Black Dye",
    # Clothes
    4259: "Hakoan Top",
    4260: "Hakoan Trousers",
    # Badges
    4006: "Beetle Badge (Hakoa)",
    4009: "Cartographer's Badge (Hakoa)",
    4020: "Guard's Badge (Hakoa)",
    4026: "Machinist Badge (Hakoa)",
}

sodic_waste_locations = {
    4305: "Sodic Waste Map",
    4974: "Hicaric Ring (Sodic Waste)",
    # Insects
    4728: "Salt Butterfly",
    # Fish
    4747: "Shelled Cup",
    # Palettes
    4423: "Neon Flash Dye",
    # Badges
    4011: "Cartographer's Badge (Sodic Waste)",
    4015: "Climbing Badge (Sodic Waste)",
    4018: "Entertainer's Badge (Sodic Waste)",
}

location_name_to_id = get_location_name_to_id()
