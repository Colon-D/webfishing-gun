extends Node

onready var Lure = get_node("/root/SulayreLure")

const ID = "webfishing_gun"

func _ready():
	Lure.add_content(ID, "webfishing_gun", "mod://Resources/Item_Tools/webfishing_gun.tres", [Lure.LURE_FLAGS.SHOP_BEACH, Lure.LURE_FLAGS.FREE_UNLOCK])
