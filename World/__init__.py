from BaseClasses import Region
from worlds.AutoWorld import World
from .items import SableItem, get_classification, item_name_to_id, ingame_name_to_display, \
    get_random_filler, item_name_groups, filler_names
from .locations import location_name_to_id, SableLocation
from .options import SableOptions


class SableWorld(World):
    """TODO: Add Description"""
    game = "Sable"
    options_dataclass = SableOptions
    options: SableOptions

    item_name_to_id = item_name_to_id
    location_name_to_id = location_name_to_id
    item_name_groups = item_name_groups

    base_id=4001

    def create_item(self, name: str) -> SableItem:
        id = item_name_to_id[name]
        return SableItem(name, get_classification(id), id, self.player)

    def create_items(self) -> None:
        items_added = 0
        for item in map(self.create_item, item_name_to_id):
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

        main_region = Region("Midden", self.player, self.multiworld)

        for loc in self.location_name_to_id.keys():
            main_region.locations.append(
                SableLocation(self.player, loc, self.location_name_to_id[loc], main_region))

        self.multiworld.regions.append(main_region)
        menu_region.connect(main_region)