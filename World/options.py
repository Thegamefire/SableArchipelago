from dataclasses import dataclass
from Options import PerGameCommonOptions, Choice, DeathLink, Toggle


class Goal(Choice):
    """Choose the end goal.
    gliding: Choose your mask and complete the gliding.
    all_masks: Collect every mask in the game.
    """
    display_name = "Goal"
    option_gliding = 0
    option_all_masks = 0
    default = 0

class DeathLinkMode(Choice):
    """What to do when a deathlink is received
    empty_stamina: Makes you exhausted.
    fasttravel_last_location: Teleports you to the last named location you visited.
    """
    display_name = "DeathLinkMode"
    option_stamina = 0
    option_fasttravel = 1
    default = 1

class RandomizeFish(Toggle):
    """Adds Fish to the randomizer"""
    display_name = "Randomize Fish"

@dataclass
class SableOptions(PerGameCommonOptions):
    death_link: DeathLink
    death_link_mode: DeathLinkMode