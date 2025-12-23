from dataclasses import dataclass
from Options import PerGameCommonOptions, Choice, DeathLink


class Goal(Choice):
    """Choose the end goal.
    gliding: Choose your mask and complete the gliding.
    """
    display_name = "Goal"
    option_gliding = 0
    default = 0

class DeathLinkMode(Choice):
    """What to do when a deathlink is received
    empty_stamina: Makes you exhausted.
    fasttravel_last_location: Teleports you to the last named location you visited.
    """
    display_naem = "DeathLinkMode"
    option_stamina = 0
    option_fasttravel = 1
    default = 1

@dataclass
class SableOptions(PerGameCommonOptions):
    death_link: DeathLink
    death_link_mode: DeathLinkMode