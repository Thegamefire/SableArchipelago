import random
from typing import List

from BaseClasses import Region, CollectionState, MultiWorld
from worlds.AutoWorld import World
from .items import SableItem, get_classification, item_name_to_id, ingame_name_to_display, \
    get_random_filler, item_name_groups, filler_names
from .locations import location_name_to_id, SableLocation
from .options import SableOptions, RandomizeMasks


class SableWorld(World):
    """TODO: Add Description"""
    game = "Sable"
    options_dataclass = SableOptions
    options: SableOptions

    item_name_to_id = item_name_to_id
    location_name_to_id = location_name_to_id
    item_name_groups = item_name_groups


    base_id=4001

    def __init__(self, multiworld: "MultiWorld", player: int):
        super().__init__(multiworld, player)
        self.mask_locations: list[SableLocation] = []
        self.mask_items: list[SableItem] = []

    def create_item(self, name: str) -> SableItem:
        item_id = item_name_to_id[name]
        item = SableItem(name, get_classification(item_id), item_id, self.player)
        if item_id in items.masks:
            self.mask_items.append(item)
        return item

    def create_items(self) -> None:
        items_added = 0
        for item in map(self.create_item, item_name_to_id):
            if item in self.mask_items and self.options.randomize_masks == RandomizeMasks.option_shuffle:
                # Don't add to item pool as placed manually
                items_added+=1
                continue
            if item.name not in filler_names:
                self.multiworld.itempool.append(item)
                items_added+=1
        for _ in range(164):
            self.multiworld.itempool.append(self.create_item(ingame_name_to_display["Chum"]))
            items_added+=1
        for _ in range(6):
            self.multiworld.itempool.append(self.create_item(ingame_name_to_display["ChumTear"]))
            items_added+=1
            self.multiworld.itempool.append(self.create_item(ingame_name_to_display["AnAncientRaceKeyItem"]))
            items_added+=1

        junk = len(location_name_to_id)-items_added
        self.multiworld.itempool += [self.create_filler() for _ in range(junk)]

    def get_filler_item_name(self):
        return get_random_filler()

    def create_regions(self) -> None:
        menu_region = Region("Menu", self.player, self.multiworld)
        self.multiworld.regions.append(menu_region)

        # TODO: Place Chums in Respective Areas
        for i in range(1, 166):  # 4801-4965
            menu_region.locations.append(
                SableLocation(self.player, f"Chum {i}", 4800+i, menu_region)
            )
        for id, loc in locations.any_region_locations.items():  # 4801-4965
            menu_region.locations.append(
                SableLocation(self.player, loc, id, menu_region)
            )
            
        for id, loc in locations.masks["any"].items():
            location = SableLocation(self.player, loc, id, menu_region)
            menu_region.locations.append(
                location
            )
            self.mask_locations.append(location)

        # Ewer (Tutorial Area)
        ewer_region = Region("Ewer", self.player, self.multiworld)
        for id, loc in locations.ewer_locations.items():
            ewer_region.locations.append(
                SableLocation(self.player, loc, id, ewer_region)
            )
        for id, loc in locations.masks["ewer"].items():
            location = SableLocation(self.player, loc, id, ewer_region)
            ewer_region.locations.append(
                location
            )
            self.mask_locations.append(location)

        # Sansee
        sansee_region = Region("Sansee", self.player, self.multiworld)
        for id, loc in locations.sansee_locations.items():
            sansee_region.locations.append(
                SableLocation(self.player, loc, id, sansee_region)
            )
        for id, loc in locations.masks["sansee"].items():
            location = SableLocation(self.player, loc, id, sansee_region)
            sansee_region.locations.append(
                location
            )
            self.mask_locations.append(location)

        # The Wash
        wash_region = Region("The Wash", self.player, self.multiworld)
        for id, loc in locations.wash_locations.items():
            wash_region.locations.append(
                SableLocation(self.player, loc, id, wash_region)
            )
        for id, loc in locations.masks["wash"].items():
            location = SableLocation(self.player, loc, id, wash_region)
            wash_region.locations.append(
                location
            )
            self.mask_locations.append(location)

        # Hakoa
        hakoa_region = Region("Hakoa", self.player, self.multiworld)
        for id, loc in locations.hakoa_locations.items():
            hakoa_region.locations.append(
                SableLocation(self.player, loc, id, hakoa_region)
            )

        # Redsee
        redsee_region = Region("Redsee", self.player, self.multiworld)
        for id, loc in locations.redsee_locations.items():
            redsee_region.locations.append(
                SableLocation(self.player, loc, id, redsee_region)
            )
        for id, loc in locations.masks["redsee"].items():
            location = SableLocation(self.player, loc, id, redsee_region)
            redsee_region.locations.append(
                location
            )
            self.mask_locations.append(location)

        # Sodic Waste
        sodic_waste_region = Region("Sodic Waste", self.player, self.multiworld)
        for id, loc in locations.sodic_waste_locations.items():
            sodic_waste_region.locations.append(
                SableLocation(self.player, loc, id, sodic_waste_region)
            )

        # Badlands
        badlands_region = Region("Badlands", self.player, self.multiworld)
        for id, loc in locations.badlands_locations.items():
            badlands_region.locations.append(
                SableLocation(self.player, loc, id, badlands_region)
            )

        self.multiworld.regions.append(ewer_region)
        self.multiworld.regions.append(sansee_region)
        self.multiworld.regions.append(wash_region)
        self.multiworld.regions.append(hakoa_region)
        self.multiworld.regions.append(redsee_region)
        self.multiworld.regions.append(sodic_waste_region)
        self.multiworld.regions.append(badlands_region)
        menu_region.connect(ewer_region)
        sansee_region.connect(ewer_region)

        ewer_region.connect(sansee_region, "Ewer -> Sansee", lambda state:  state.has("Sansee Map", self.player))
        wash_region.connect(sansee_region, "The Wash -> Sansee", lambda state:  state.has("Sansee Map", self.player))
        redsee_region.connect(sansee_region, "Redsee -> Sansee", lambda state:  state.has("Sansee Map", self.player))
        sodic_waste_region.connect(sansee_region, "Sodic Waste -> Sansee", lambda state:  state.has("Sansee Map", self.player))
        badlands_region.connect(sansee_region, "Badlands -> Sansee", lambda state:  state.has("Sansee Map", self.player))

        sansee_region.connect(wash_region, "Sansee -> The Wash", lambda state: state.has("The Wash Map", self.player))
        badlands_region.connect(wash_region, "Badlands -> The Wash", lambda state: state.has("The Wash Map", self.player))

        redsee_region.connect(hakoa_region, "Redsee -> Hakoa", lambda state: state.has("Hakoa Map", self.player))
        badlands_region.connect(hakoa_region, "Badlands -> Hakoa", lambda state: state.has("Hakoa Map", self.player))

        sansee_region.connect(redsee_region, "Sansee -> Redsee", lambda state: state.has("Redsee Map", self.player))
        hakoa_region.connect(redsee_region, "Hakoa -> Redsee", lambda state: state.has("Redsee Map", self.player))
        sodic_waste_region.connect(redsee_region, "Sodic Waste -> Redsee", lambda state: state.has("Redsee Map", self.player))
        badlands_region.connect(redsee_region, "Badlands -> Redsee", lambda state: state.has("Redsee Map", self.player))

        sansee_region.connect(sodic_waste_region, "Sansee -> Sodic Waste", lambda state: state.has("Sodic Waste Map", self.player))
        redsee_region.connect(sodic_waste_region, "Redsee -> Sodic Waste", lambda state: state.has("Sodic Waste Map", self.player))

        sansee_region.connect(badlands_region, "Sansee -> Badlands", lambda state: state.has("Badlands Map", self.player))
        wash_region.connect(badlands_region, "The Wash -> Badlands", lambda state: state.has("Badlands Map", self.player))
        hakoa_region.connect(badlands_region, "Hakoa -> Badlands", lambda state: state.has("Badlands Map", self.player))
        redsee_region.connect(badlands_region, "Redsee -> Badlands", lambda state: state.has("Badlands Map", self.player))


        assert len(location_name_to_id) == sum(len(region.locations) for region in self.multiworld.get_regions(self.player))

    def pre_fill(self) -> None:
        if self.options.randomize_masks == RandomizeMasks.option_shuffle:
            random.shuffle(self.mask_items)
            for i, item in enumerate(self.mask_items):
                self.mask_locations[i].place_locked_item(item)
