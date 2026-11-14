# Connection: CONN_MC_SAM (Middle Column to Side Angle Column Module, dowelled)

## Scene Instances
| Role | Part ID | Scene Object |
| --- | --- | --- |
| Column | MC | Middle Column 1 |
| Angle Module | SAM | Side Andgle Module 2 (SAM2) |
| Dowels | DOW | SAM2 > Dowels2 > Object.2501, Object.2502, Object.2503 |

Part IDs identify part *types*. This connection refers to specific *instances* in `MC1Scene`, listed above.

## Paticipating parts and interfaces
| Interface | Parts | How they fit | Source |
| --- | --- | --- | --- |
| A | MC and SAM mortise | The column end is slid into the angle module's mortise. | Manual, step 4 |
| B | Dowels (3) and aligned holes through MC and SAM | Three 1" dowels pass though holes in both pieces, locking the column in the model. | Manual, step 4 |

## Dimension Check
| Object Checked | Method | Documented value | Result |
| --- | --- | --- | --- | --- |
| Object.2503 (Dowel) | `sizeVerification` script, Console log | 1" (0.0254 m) diameter, Final Report | Matches (0.0254 m) |

Other parts not checked idividually.

## Explanatory views (planned)
- **Assembled:** all three participants in their original positions, highlighted, with interface markers A and B visible.
- **Exploded:** the column slides out of SAM along the mortice axis; the dowels withdraw along the hole axis. Labelled "Explanatory visualization - not to scale".
- Return to the structure restores the original positions exactly.

