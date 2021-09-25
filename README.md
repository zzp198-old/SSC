|           字段名            |       类型        |               原版同步频率               |                  备注                  |
| :-------------------------: | :---------------: | :--------------------------------------: | :------------------------------------: |
|            name             |      string       |                 进入世界                 |                无需保存                |
|         difficulty          |        int        |                 进入世界                 |                无需保存                |
|            hair             |        int        |             进入世界，发型师             |             存储格式为byte             |
|    hideVisibleAccessory     |      bool[]       |                   实时                   |                                        |
|          hideMisc           |     BitsByte      |             进入世界，梳妆台             |             存储格式为byte             |
|         skinVariant         |        int        |             进入世界，梳妆台             |                                        |
|          statLife           |        int        |                   实时                   |                                        |
|         statLifeMax         |        int        |                   实时                   |                                        |
|          statMana           |        int        |                   实时                   |                                        |
|         statManaMax         |        int        |                   实时                   |                                        |
|       extraAccessory        |       bool        |            进入世界，恶魔之心            |                                        |
|    unlockedBiomeTorches     |       bool        |           进入世界，火把神徽章           |                                        |
|      UsingBiomeTorches      |       bool        |           进入世界，火把神徽章           |                                        |
| downedDD2EventAnyDifficulty |       bool        |            进入世界，旧日军团            |                                        |
|          taxMoney           |        int        |                                          |                                        |
|          hairColor          |       Color       |             进入世界，发型师             |             存储格式为int              |
|          skinColor          |       Color       |          进入世界，梳妆台(bug)           |             存储格式为int              |
|          eyeColor           |       Color       |          进入世界，梳妆台(bug)           |             存储格式为int              |
|         shirtColor          |       Color       | 进入世界，发型师(bug,但确实是发型师修改) |             存储格式为int              |
|       underShirtColor       |       Color       |          进入世界，梳妆台(bug)           |             存储格式为int              |
|         pantsColor          |       Color       |          进入世界，梳妆台(bug)           |             存储格式为int              |
|          shoeColor          |       Color       |          进入世界，梳妆台(bug)           |             存储格式为int              |
|            armor            |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|             dye             |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|         miscEquips          |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|          miscDyes           |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|          inventory          |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|            bank             |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|            bank2            |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|            bank3            |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|            bank4            |      Item[]       |                   实时                   | 需额外保存,存储格式为List<TagCompound> |
|          taxMoney           |        int        |                  不同步                  |                                        |
|        voidVaultInfo        |     BitsByte      |                   实时                   |             存储格式为byte             |
|             spX             |       int[]       |                  不同步                  |                                        |
|             spY             |       int[]       |                  不同步                  |                                        |
|             spI             |       int[]       |                  不同步                  |                                        |
|             spN             |     string[]      |                  不同步                  |                                        |
|          hbLocked           |       bool        |                  不同步                  |                                        |
|          hideInfo           |      bool[]       |                  不同步                  |                                        |
|    anglerQuestsFinished     |        int        |                  不同步                  |                                        |
|         DpadRadial          |       int[]       |             未知(当作不同步)             |                                        |
|      builderAccStatus       |       int[]       |                  不同步                  |                                        |
|      bartenderQuestLog      |        int        |                  不同步                  |                                        |
|            dead             |       bool        |                   实时                   |                                        |
|        respawnTimer         |        int        |                   实时                   |                                        |
|   lastTimePlayerWasSaved    |       long        |                  不同步                  |                                        |
|   golferScoreAccumulated    |        int        |                  不同步                  |                                        |
|       creativeTracker       |      byte[]       |                  不同步                  |                                        |
|       Main.mouseItem        |       Item        |                  不同步                  |                                        |
|       Main.guideItem        |       Item        |                  不同步                  |                                        |
|      Main.reforgeItem       |       Item        |                  不同步                  |                                        |
|      Main.CreativeMenu      |       Item        |                  不同步                  |                                        |
|    CreativePowerManager     |      byte[]       |                  不同步                  |                                        |
|           hairDye           |      string       |                  不同步                  |                                        |
|          research           | List<TagCompound> |                  不同步                  |                                        |
|           modData           | List<TagCompound> |                  不同步                  |                                        |
|          modBuffs           | List<TagCompound> |                  不同步                  |                                        |
|        infoDisplays         |   List<string>    |                  不同步                  |                                        |
|          usedMods           |   List<string>    |                  不同步                  |                                        |

