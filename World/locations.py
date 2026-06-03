from BaseClasses import Location
from . import items

class SableLocation(Location):
    game = "Sable"

def get_location_name_to_id():
    table = items.item_name_to_id.copy()

    del table[items.ingame_name_to_display["Chum"]]
    del table[items.ingame_name_to_display["ChumTear"]]
    del table[items.ingame_name_to_display["AnAncientRaceKeyItem"]]

    for i in range (1, 166): # 4801-4965
        table[f"Chum {i}"]=4800+i
    for i in range(1, 7): # 4966-4971
        table[f"Chum Tear {i}"]=4800+165+i

    for i, area in enumerate(["Sansee", "Redsee","Sodic Waste","Badlands","Hakoa","The Wash"]):
        table[f"Hicaric Ring ({area})"]=4800+165+7+i

    assert len(list(map(lambda x: x, table.values()))) == len(set(table.values()))

    return table

location_name_to_id = get_location_name_to_id()