opentk-sdl2
===========

An (unfinished, experimental) SDL2 backend for OpenTK.

This is a fork of opentk with an SDL2 backend. It was an experiment, and
you're better off using https://github.com/flibitijibibo/SDL2-CS for your
.NET SDL needs. There's an accompanying MonoGame branch, and there are plans to
integrate the SDL2# MonoGame backend into MonoGame master.

Please feel free to steal code from this branch if you want to experiment with stuff
here, but be warned:

* As-is, the codebase is tied to a custom version of SDL2 (with a special patch applied to change some fullscreen behaviour).
* Some things (like Game Controller support) were never finished and tested.
* There are some pretty nasty hacks, as this was my first real incursion into the world of C#.
* Everything is pretty out-of-date.
* Thus far everything is pretty Linux specific, as that's the platform I'm developing on. It shouldn't be too hard to get it to work on OS X or Windows, though.

What does work:

* Multithreaded OpenGL! (No guarantee that your GL driver doesn't suck though)
* Enough stuff to make it better than the old OpenTK platform backends.
* A bunch of other fixes in the associated MonoGame branch (a new Song backend, etc)
* As a proof-of-concept, clearly it went well as everyone's porting their opentk stuff to SDL2 now! :)

If you're keen on looking at this some more, there is some useful info in issue #2.

Have fun,

-- David
