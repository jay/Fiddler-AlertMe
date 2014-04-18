Fiddler-AlertMe
===============

Alert when a user specified regex matches data in the web debugging proxy Fiddler.

Why..
-----

I wrote this so that I could be alerted of exceptions (regex `(?i)exception`) in Fiddler's event log. **It is a proof of concept only and not for production.** Unfortunately I don't have the time to do any further development.

[Thanks to Eric Lawrence and Telerik for making Fiddler available for free.](http://www.telerik.com/fiddler)

What can it do?
---------------

* Monitor requests, responses, event log
* Alert by sound, e-mail, message box
* User can specify sound, e-mail, hosts to watch, regular expression

The code is functioning as expected and is a good proof of concept. Several issues still need to be addressed, most importantly latency, blocking and a lack of rate limiting (imagine a match-any regex triggering thousands of emails due to no rate limiting).

Other
-----


### License

AlertMe is free software and it is licensed under the [GNU Lesser General Public License version 3 (LGPLv3)](http://www.gnu.org/licenses/lgpl-3.0.html), a license that will keep it free. You may not remove my copyright or the copyright of any contributors under the terms of the license. The source code for AlertMe can be used in proprietary software as long as it's packaged separately (eg compiled as an LGPLv3+ library). **In any case please review the GPLv3 and LGPLv3 licenses, which are designed to protect freedom, not take it away.**

### Source

The source can be found on [GitHub](https://github.com/jay/Fiddler-AlertMe). Since you're reading this maybe you're already there?

### Send me any questions you have

Jay Satiro `<raysatiro$at$yahoo{}com>` and put CSVread/CSVwrite in the subject.
