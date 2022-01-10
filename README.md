
# ag.WPF.ColorPicker

one|two
---|---
first|second

A custom WPF controls library allowing user to choose a color. Two different packages are available:

1. .NET Framework (version >= 4.7.2)
2. .NET (version >= 6).

A library supplies two types of control: ColorPanel - always fully open, and ColorPicker, where panel is opening as drop-down part.

| ![ColorPanel](https://am3pap005files.storage.live.com/y4mPdl1eiaLbfsGstYHwuJem9kHxw5eR8x-s_SjjoALPG_bVMH3TUwKZ9kDvumicFXWY6BMdi-cTTO7bezEt5YrJ94Lr0liZ7mg7FNy904QFCfB4W8IE0F1g3irdHdk-3wwb5ty7cQAV2iR9nFW9fAZq9KBXO8zesYyRhTwLiwH1CYuSTWTLx-pv63u1yNGMFsu?width=414&height=410&cropmode=none "ColorPanel") | ![ColorPicker](https://am3pap005files.storage.live.com/y4m_hu2fVsNBwqh71j_erKIrD_sM79HexWXFkpdkQqmIyFtfrZnUXxx8DaviCgrWNby1o4tbHuHqP6MZBVm8Inn2c_aDPfmln5KMrhzh-G2HX6CoOPzTfKxMsetOQpuTF5hikL1dEpRpuxns44gSW4TlvfJwCk-I8fUbJPb9H2JSKujYGzUlWlHqbdQR3QQFsza?width=375&height=422&cropmode=none "ColorPicker") |
| :----------------------------------------------------------: | :----------------------------------------------------------: |

## Usage

Add ag.WPF.ColorPicker namespace to XAML file:

```csharp
xmlns:picker="clr-namespace:ag.WPF.ColorPicker;assembly=ag.WPF.ColorPicker"
```

Add ColorPicker

```csharp
<picker:ColorPicker x:Name="_picker"/>
```

## Installation

Use NuGet package manager.

- first
- second

## ColorPanel

### Enumerations

```csharp
public enum PanelView
```

Represents available ColorPanel views

Field|Value|Description
------|-----|-----------
Standard|0|Standard view with full controls set available
Simple|1|Simple minimalistic view

Simple ColorPicker</br>
![Simple](https://am3pap005files.storage.live.com/y4m8Kpg_wp_PECb1y94lgoCohPDltyqBbEyuS3O7I1-Sp10pDmQOe0lK6q2U9jlEmYvaPws70aqq2kmc-Ju2JiMfryO5jmTCSIpSVSwQHk4W3pHjAhBKbsSJYyDI8KjEQHgt_gnzHuPhA8M8RX8642N3HF7FOH_5y7MCQV77FZUf2-JUBUbT4Blna5BxbBx_diS?width=376&height=218&cropmode=none)
### Properties

```csharp
public Color SelectedColor
```

Gets or sets selected color.

```csharp
public string ColorString
```

Gets selected color's string representation.

### Attached properties

```csharp
public PanelView ColorPanel.PanelView
```

Gets or sets a value that indicates the view of *ColorPanel*. Default value is PanelView.Standard

```csharp
public string ColorPanel.TitleCancel
```

Gets or sets a value that indicates the title of *Cancel* button. Default value is "Cancel".

```csharp
public string ColorPanel.TitleApply
```

Gets or sets a value that indicates the title of *Apply* button. Default value is "Apply".

```csharp
public string ColorPanel.TitleShadesAndTints
```

Gets or sets a value that indicates the title of *Shades and tints* group box. Default value is "Shades and tints".

```csharp
public string ColorPanel.TitleColorModes
```

Gets or sets a value that indicates the title of *Color modes* group box. Default value is "Color modes".

```csharp
public string ColorPanel.TitleFormat
```

Gets or sets a value that indicates the title of *Format* text box. Default value is "Format".

```csharp
public string ColorPanel.TitleTabStandard
```

Gets or sets a value that indicates the title of *Standard* tab. Default value is "Standard".

```csharp
public string ColorPanel.TitleTabBasic
```

Gets or sets a value that indicates the title of *Basic* tab. Default value is "Basic".

```csharp
public string ColorPanel.TitleTabCustom
```

Gets or sets a value that indicates the title of *Custom* tab. Default value is "Custom".

```csharp
public bool ColorPanel.ShowCommandsPanel
```

Gets or sets a value that indicates whether *Apply* and *Cancel* buttons are shown. Default value is True.

### Events

```csharp
public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
```

Occurrs when SelectedColor property is changed.</br> Raises only if *ShowCommandsPanel* property is set to False.</br>
*OldValue* field of *RoutedPropertyChangedEventArgs* contains an initial color.</br>*NewValue* of RoutedPropertyChangedEventArgs contains a new color.

```csharp
public event RoutedPropertyChangedEventHandler<Color> ColorApplied
```

Occurrs when user click on *Apply* button.</br>
*OldValue* field of *RoutedPropertyChangedEventArgs* contains an initial color.</br>*NewValue* of RoutedPropertyChangedEventArgs contains a new color.

```csharp
public event RoutedEventHandler ColorCanceled
```

Occurrs when user click on *Cancel* button.

## ColorPicker

### Properties

```csharp
public Color SelectedColor
```

Gets or sets selected color.

```csharp
public string ColorString
```

Gets selected color's string representation.

### Events

```csharp
public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
```

Occurrs when SelectedColor property is changed.
