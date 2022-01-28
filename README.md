
# ag.WPF.ColorPicker

.NET Framework (4.7.2) | .NET (6)
--- | ---
first | second

Custom WPF controls that allows a user to pick a color from a predefind color palettes and screen.

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

![ColorPicker](https://am3pap005files.storage.live.com/y4maighgBk-dHO_mZbCOzeE3AF9Uh_S2r2JK8AK8UqWdfSotBX-pAKy2lCcOzPSt5hjgEEHr3UjrxDAEKuqGErbJ_GWbwLVIYPdKtgs9GAyFDQDpzn6AgypYPrR25wTsLOYBjzXckIE6LESWJOXvQpUAOOHY8huXYgHsmRkh2TaaJNrgiIa9vvK6bsYu9hX4L1g?width=501&height=563&cropmode=none "ColorPicker")</br>
Simple ColorPicker</br>
![Simple](https://am3pap005files.storage.live.com/y4mIY3jatPSuW16w-gIRURIEGf1iE8yQYO2gzoftZROAcgg0363n1-t7HwLo34FMNef6jZzqZb7v64Aj8lrKnANd92NSaFghiGAQeLhIPx3F8tz19TLmQHAU9hmQccDD3QUbg3gnT2pUeocL3pYv2T3_hAxYe0CnUb5NFhWfLI-jeNeKw_8yT-HNK0nKSYgrRhk?width=501&height=291&cropmode=none)</br>
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

![ColorPanel](https://am3pap005files.storage.live.com/y4mI2M-S3zPUj1M0ZaUuZjv5UlyAePbJRPc0FuqjaiYGUxdBKvPgM650JglQfo5eAH8463Mvx16nU79FtBHecZte8bLGlhzIMGjJVAfxGq7hFq0fzbL0Xvw_4qpEPUAL6IjryQoHOPFg2eNx0cyrVkCYPA2tMcaQZnbYdp_oiL3_YjqQozQuSITVoJYSyo5CXkY?width=545&height=544&cropmode=none "ColorPanel")

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
