
# ag.WPF.ColorPicker

.NET Framework (4.7.2) | .NET (6)
--- | ---
first | second

Custom WPF controls that allows a user to pick a color from a predefind color palettes and/or screen.

## Installation

Use NuGet package manager.

.NET Framework (4.7.2) | .NET (6)
--- | ---
first | second

## Usage

Add ag.WPF.ColorPicker namespace to XAML file:

```csharp
xmlns:picker="clr-namespace:ag.WPF.ColorPicker;assembly=ag.WPF.ColorPicker"
```

Add ColorPicker

```csharp
<picker:ColorPicker x:Name="_picker"/>
```

## Enumerations

### PanelView

Represents available ColorPanel views

Field|Value|Description
------|-----|-----------
Standard|0|Standard view with full controls set available
Simple|1|Simple minimalistic view

## ColorPicker

![ColorPicker](https://am3pap005files.storage.live.com/y4m_hu2fVsNBwqh71j_erKIrD_sM79HexWXFkpdkQqmIyFtfrZnUXxx8DaviCgrWNby1o4tbHuHqP6MZBVm8Inn2c_aDPfmln5KMrhzh-G2HX6CoOPzTfKxMsetOQpuTF5hikL1dEpRpuxns44gSW4TlvfJwCk-I8fUbJPb9H2JSKujYGzUlWlHqbdQR3QQFsza?width=375&height=422&cropmode=none "ColorPicker")</br>
Simple ColorPicker</br>
![Simple](https://am3pap005files.storage.live.com/y4m8Kpg_wp_PECb1y94lgoCohPDltyqBbEyuS3O7I1-Sp10pDmQOe0lK6q2U9jlEmYvaPws70aqq2kmc-Ju2JiMfryO5jmTCSIpSVSwQHk4W3pHjAhBKbsSJYyDI8KjEQHgt_gnzHuPhA8M8RX8642N3HF7FOH_5y7MCQV77FZUf2-JUBUbT4Blna5BxbBx_diS?width=376&height=218&cropmode=none)
### Properties

Property | Type | Description | Default value
--- | --- | --- | ---
SelectedColor | Color | Gets or sets selected color | Red
ColorString | string | Gets selected color's string representation |

### Events

Event | Description | Remarks
--- | --- | ---
SelectedColorChanged | Occurrs when SelectedColor property is changed | *OldValue* field of *RoutedPropertyChangedEventArgs* contains an initial color. *NewValue* of RoutedPropertyChangedEventArgs contains a new color.

## ColorPanel

![ColorPanel](https://am3pap005files.storage.live.com/y4mPdl1eiaLbfsGstYHwuJem9kHxw5eR8x-s_SjjoALPG_bVMH3TUwKZ9kDvumicFXWY6BMdi-cTTO7bezEt5YrJ94Lr0liZ7mg7FNy904QFCfB4W8IE0F1g3irdHdk-3wwb5ty7cQAV2iR9nFW9fAZq9KBXO8zesYyRhTwLiwH1CYuSTWTLx-pv63u1yNGMFsu?width=414&height=410&cropmode=none "ColorPanel")

### Properties

Property | Type | Description | Default value
--- | --- | --- | ---
SelectedColor | Color | Gets or sets selected color | Red
ColorString | string | Gets selected color's string representation |

### Attached properties

Property | Type | Description | Default value
--- | --- | --- | ---
PanelView | PanelView | Gets or sets a value that indicates the view of *ColorPanel* | PanelView.Standard
TitleCancel | string | Gets or sets a value that indicates the title of *Cancel* button | Cancel
TitleApply | string | Gets or sets a value that indicates the title of *Apply* button | Apply
TitleShadesAndTints | string | Gets or sets a value that indicates the title of *Shades and tints* group box | Shades and tints
TitleColorModes | string | Gets or sets a value that indicates the title of *Color modes* group box | Color modes
TitleFormat | string | Gets or sets a value that indicates the title of *Format* text box | Format
TitleTabStandard | string | Gets or sets a value that indicates the title of *Standard* tab | Standard
TitleTabBasic | string | Gets or sets a value that indicates the title of *Basic* tab | Basic
TitleTabCustom | string | Gets or sets a value that indicates the title of *Custom* tab | Custom
ShowCommandsPanel | bool | Gets or sets a value that indicates whether *Apply* and *Cancel* buttons are shown | True


### Events

Event | Description | Remarks
--- | --- | ---
SelectedColorChanged | Occurrs when SelectedColor property is changed |  Raises only if *ShowCommandsPanel* property is set to False. *OldValue* field of *RoutedPropertyChangedEventArgs* contains an initial color. *NewValue* of RoutedPropertyChangedEventArgs contains a new color.
ColorApplied | Occurrs when user click on *Apply* button | *OldValue* field of *RoutedPropertyChangedEventArgs* contains an initial color. *NewValue* of *RoutedPropertyChangedEventArgs* contains a new color.
ColorCanceled | Occurrs when user click on *Cancel* button
