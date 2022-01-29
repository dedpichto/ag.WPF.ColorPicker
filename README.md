
# ag.WPF.ColorPicker

first

Custom WPF controls that allows a user to pick a color from a predefind color palettes and screen.

## Installation

Use NuGet package manager.

first

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

![ColorPicker](https://am3pap005files.storage.live.com/y4mf2W_o2lI8j-Zx_nIXBGRYCzXsAp2Bj9escVmQyCFQOAKcDImrHR0in63IXhZaMeuQnDBdBFt5d3r5zJHpxSjAlNAhRmKcFCSTJWfPljYGZni80fud7F62V7PfRYLnm92CaZsSpsNEpqopXGwTId2DkpFfd7yFPn1hs5ACo-iCCZfnOwtKeTXUh_t07aIIhLF?width=220&height=32&cropmode=none "ColorPicker")</br>

![ColorPicker](https://am3pap005files.storage.live.com/y4m0Tb-wnFOxqyQ2oENXhcpyB9t2NHbypbhiLDalMePEEIJ-B4lIhkJFwnby_IvMspeG-tOxr9nAdYL2rWof5jVCXIjgxsqSZac44NMMV8lSDYPUfLtiEuFRRZVRL9FAO4tYOi1690XCc3cI0xiiJscNDn6eqNla7OeliXrP1pBQzNVDGTjVmqAiKSvgU1W0LVM?width=503&height=606&cropmode=none "ColorPicker")</br>

Simple ColorPicker</br>
![Simple](https://am3pap005files.storage.live.com/y4m_XgfKX4jiWxAc8FAypyq5RGSJRzZDwavGH1NrgahSuo-hyI1caDxna60MhUGCYpV6Ja88BcKzGk9uTsMI2ArQbfsyHncvK5cavCbhtEl-ujYafGUbVQeVtogc12jRb5ejgqkAqqKxesNc1h4FE3ToKn-dZLtDpy2S-pbdjJGSFrP-0WBKpjNXQY3-I8LgdCl?width=503&height=334&cropmode=none)</br>

Styled ColorPicker</br>
![Styled](https://am3pap005files.storage.live.com/y4mroTj9anmehNNgT1WeuYbcJvrSs-D6I4FMuKCwJVfRb6spyNbrayJzvCxFLzhK1wOz2gDFFXPKVAIm_OXSm178tg3t9qPRBvdEvGK-zfBz24JfJ7M67QBt1k2n8Bj_58OI_WXb7DNTPCNVCiuwKARAWQ-D6il32PCPv77WVjLBC7rnVI1PE2SrpCjQtmRWOmn?width=506&height=611&cropmode=none)

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

![ColorPanel](https://am3pap005files.storage.live.com/y4mL4QjQXZZsx60HQEnqI1zEgi9kgme0g-wsvcwbKPiARc5RPa94S7SJfbi4ThKEttRIKeVOnvtTi8mtbIrikTW34OGXo9olfBJIrpGBsGdGARB5TX6oiGW3gafzf0ndTmT6sIjTVne1LIuxbCBiF0td0FOoRSnqChKVA6Kw0ORNZxF0GjhdqCYMDAkYMmaNQlA?width=546&height=545&cropmode=none "ColorPanel")

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

### Events

Event | Description | Remarks
--- | --- | ---
SelectedColorChanged | Occurrs when SelectedColor property is changed |  *OldValue* field of *RoutedPropertyChangedEventArgs* contains an initial color. *NewValue* of RoutedPropertyChangedEventArgs contains a new color.
