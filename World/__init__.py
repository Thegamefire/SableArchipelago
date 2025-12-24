from worlds.AutoWorld import World
from worlds.Sable.items import SableItem, get_classification, item_name_to_id, ingame_name_to_display, \
    get_random_filler, item_name_groups
from worlds.Sable.options import SableOptions


class SableWorld(World):
    """TODO: Add Description"""
    game = "Sable"
    options_dataclass = SableOptions
    options: SableOptions

    item_name_to_id = item_name_to_id
    item_name_groups = item_name_groups

    def create_item(self, name: str) -> SableItem:
        id = item_name_to_id[name]
        return SableItem(name, get_classification(id), id, self.player)

    def create_items(self) -> None:
        for item in map(self.create_item, item_name_to_id):
            self.multiworld.itempool.append(item)
        for _ in range(164):
            self.multiworld.itempool.append(self.create_item(ingame_name_to_display["Chum"]))
        for _ in range(6):
            self.multiworld.itempool.append(self.create_item(ingame_name_to_display["ChumTear"]))
            self.multiworld.itempool.append(self.create_item(ingame_name_to_display["AnAncientRaceKeyItem"]))

        # itempool and number of locations should match up.
        # If this is not the case we want to fill the itempool with junk.
        junk = 0  # TODO: calculate this based on player options
        self.multiworld.itempool += [self.create_item(get_random_filler()) for _ in range(junk)]