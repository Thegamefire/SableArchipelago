from BaseClasses import Location
from . import items

class SableLocation(Location):
    game = "Sable"

def get_location_name_to_id():
    table = items.item_name_to_id.copy()

    del table[items.ingame_name_to_display["Chum"]]
    del table[items.ingame_name_to_display["ChumTear"]]
    del table[items.ingame_name_to_display["AnAncientRaceKeyItem"]]

    for i in range (1, 166):
        table[f"Chum {i}"]=4800+i
    for i in range(1, 7):
        table[f"Chum Tear {i}"]=4800+165+i
        table[f"Hicaric Ring {i}"]=4800+165+6+i

    return table

location_name_to_id = get_location_name_to_id()