# ScrollZoom

### A simple adjustable zoom mod for desktop users!

Enables the ability to adjust your zoom level by rolling your scroll wheel up/down!

#### Additionally includes:
+ A _toggle zoom feature_ if holding the middle mouse button and scrolling is uncomfortable and you haven't rebound the key!
+ _Correctly scaling HUD_ so your notifications and reticle don't get wacky when you zoom in!
+ Your _current zoom level_ is passed to the default CVR Parameter Stream `ZoomFactor`, so you can drive animations based on *how much* you're zoomed in/out!
+ A whole bunch of _settings_ to tweak as you desire! (*See* [MelonPrefs](#melonprefs))

---

## MelonPrefs
* **Hold to Zoom** - default: `true`<br>
`true`: Standard CVR behaviour<br>
`false`: Toggle Zoom behaviour (one press to enable zoom, another press to disable)

* **Target Zoom Level** - default: `0.75`<br>
The level of zoom you want to snap to when you start zooming **if *Remember Zoom Level* is** ``false``. Relative to *Max Zoom Level*.

* **Remember Zoom Level** - default: `true`<br>
`true`: Your last 'amount of zoomed in' is saved and you'll snap back to that level when you zoom in again.<br>
`false`: You will always initially snap to your *Target Zoom Level* every time you start zooming.

* **Max Zoom Level** - default: `0.5`<br>
The maximum cap you can zoom to. *Assuming* you have the standard CVR Field of View, the default `0.5` is roughly the same as the unmodded game value.

* **+/- Amount Per Scroll** - default: `0.1`<br>
The amount per scroll wheel turn you zoom in/out. (Actual value decreases the closer you get to your max zoom level to prevent exponential jumps in zoom level!)

---

# Disclosure  

> ---
> ⚠️ **Notice!**  
>
> This mod's developer(s) and the mod itself, along with the respective mod loaders, have no affiliation with ABI!
>
> ---