# Connection Taxonomy

*Note: All parts herein should be considered *typical* parts.*

## Connection Archetypes
| Archetype Name | Description | Reference Image |
| --- | --- | --- |
| **Slat-Column With T-Pin Connection (CONN_TPIN)** | A keyed tenon and mortise joint where a slat's tenon passes all the way through a column's mortise and is held in place by a T-Pin. |<img width="664" height="294" alt="image" src="https://github.com/user-attachments/assets/c5d7b5b5-e722-45cb-a120-288dcc1c2174" />|
| **Rafter-Purlin Slot Connection (CONN_SLOT)** | A notched purlin joint, wherein the purlin is fully slotted into a notch in the rafter. | <img width="648" height="302" alt="image" src="https://github.com/user-attachments/assets/a3cf25a7-c478-4126-abd1-370076dc9db2" />|
| **Dowel Connection (CONN_DOWEL)** | A bridal joint between two rafter pieces or a rafter piece and a column, pinned by dowels. | <img width="641" height="180" alt="image" src="https://github.com/user-attachments/assets/85f00cea-16b1-4c37-9edc-2597cc14d477" />|

## Connection Variants

### Slat-Column With T-Pin Connection Variants

<table>
  <tr>
    <td>
      <table>
      <tr><td>These connections consist of >=1 T-Pin.</td></tr><tr><td>
        <table>
            <tr>
              <th>Variant Name</th>
              <th>Variant ID</th>
              <th>Component1</th>
              <th>Component2</th>
              <th>Component3</th>
            </tr>
            <tr>
              <th>End Panel-Column1 T-Pin Connection</th>
              <th>CONN_TPIN_WP_EC1</th>
              <td>Wall Panel</td>
              <td>End Column1</td>
              <td></td>
            </tr>
            <tr>
              <th>End Panel-Column2 T-Pin Connection</th>
              <th>CONN_TPIN_WP_EC2</th>
              <td>Wall Panel</td>
              <td>End Column2</td>
              <td></td>
            </tr>
            <tr>
              <th>Middle Panel-Column T-Pin Connection</th>
              <th>CONN_TPIN_2WP_MC</th>
              <td>Wall Panel</td>
              <td>Wall Panel</td>
              <td>Middle Column</td>
            </tr>
        </table>
        </td></tr>
      </table>
      <i>Technical Note: CONN_TPIN_2WP_MC will require a bilaterally-symmetrical explosion to pull both wall panels away.</i>
    </td>
    <td>
      <img width="289" height="374" alt="image" src="https://github.com/user-attachments/assets/5ae239b3-60a6-4848-8b60-05a2cce09025" />
    </td>
  </tr>
</table>

### Rafter-Purlin Slot Connection Variants

<table>
  <tr>
    <td>
      <table>
      <tr><td>These parts are friction-fit and do not use a fastener.</td></tr><tr><td>
        <table>
            <tr>
              <th>Variant Name</th>
              <th>Variant ID</th>
              <th>Component1</th>
              <th>Component2</th>
            </tr>
            <tr>
              <th>Side-Purlin Slot Connection</th>
              <td><strong>CONN_SLOT_RP_SAM</strong></td>
              <td>Side Angle Module</td>
              <td>Roof Purlin</td>
            </tr>
            <tr>
              <th>Beam-Purlin Slot Connection</th>
              <td><strong>CONN_SLOT_RP_RB</strong></td>
              <td>Roof Beam</td>
              <td>Roof Purlin</td>
            </tr>
            <tr>
              <th>Top-Purlin Slot Connection</th>
              <td><strong>CONN_SLOT_RP_TAM</strong></td>
              <td>Top Angle Module</td>
              <td>Roof Purlin</td>
            </tr>
        </table>
        </td></tr>
        </table>
    </td>
    <td>
      <img width="384" height="380" alt="image" src="https://github.com/user-attachments/assets/ad84aac4-18ff-494b-90a1-6c191415fdb8" />
    </td>
  </tr>
</table>

### Dowel Connection Variants

<table>
  <tr>
    <td>
      <table>
      <tr><td>These connections consist of >=3 dowels.</td></tr><tr><td>
        <table>
            <tr>
              <th>Variant Name</th>
              <th>Variant ID</th>
              <th>Component1</th>
              <th>Component2</th>
            </tr>
            <tr>
              <th>End Rafter-Column1 Dowel Connection</th>
              <th>CONN_DOWEL_EC1_SAM</th>
              <td>End Column1</td>
              <td>Side Angle Module</td>
            </tr>
            <tr>
              <th>End Rafter-Column2 Dowel Connection</th>
              <th>CONN_DOWEL_EC2_SAM</th>
              <td>End Column2</td>
              <td>Side Angle Module</td>
            </tr>
            <tr>
              <th>Mid Rafter-Column Dowel Connection</th>
              <th>CONN_DOWEL_MC_SAM</th>
              <td>Middle Column</td>
              <td>Side Angle Module</td>
            </tr>
            <tr>
              <th>Side-Beam Dowel Connection</th>
              <th>CONN_DOWEL_SAM_RB</th>
              <td>Side Angle Module</td>
              <td>Roof Beam</td>
            </tr>
            <tr>
              <th>Top-Beam Dowel Connection</th>
              <th>CONN_DOWEL_TAM_RB</th>
              <td>Top Angle Module</td>
              <td>Roof Beam</td>
            </tr>
        </table>
        </td></tr>
    </table>
  </tr>
  <tr>
    <td>
      <table>
        <th><img width="362" height="425" alt="image" src="https://github.com/user-attachments/assets/fb350482-4f0e-4eae-a802-85a32746e3e0" /></th>
        <td><img width="472" height="326" alt="image" src="https://github.com/user-attachments/assets/eb29055b-76f4-4ea8-a14a-75b8abc41ffd" /></td>
        <td><img width="512" height="443" alt="image" src="https://github.com/user-attachments/assets/8bc9bbda-53ac-40bf-abc0-77e16704912b" /></td>
      </table>
    </td>
  </tr>
</table>
