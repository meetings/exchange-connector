# exchange-connector

The purpose of this software is to relay calendar data from an on-premises Microsoft Exchange server to Meetin.gs backend. It can to be installed on a server which lies on-premises with the Exchange server so that no inbound traffic to the on-premises dmz is required for data relay.

The first version is developed against Exchange 2010. Exchange 2013 should be backwards compatible and work but is not tested.

For the software to work you need to have the following:

* Meetin.gs application configured to specify relayed user emails.
* Meetin.gs API key to access the application.
* Rights to access relayed user calendars with the installed service rights (maybe using an AD group).
* On-premises firewall to be configured to allow outbound HTTPS connections to domain "api.meetin.gs".
* An idea of when the Exchange is not used that much and can be hammered more for a full sync.

# Compilation

Requires .NET 4.5. (customer told us that server that will be running the service do has Framework 4.5 installed in it).

Requires Newtonsoft.Json Blend library to be installed.

TODO: instructions for compilation

# Installation and configuration

The following parameters need to be configured using a JSON file placed at C://meetings.json or in a file configured as a command line parameter

    {
      "api_domain" : "api.meetin.gs",
      // use api-dev.meetin.gs for development
      // use api.meetin.gs for production
      
      "api_key" : "1234567890abcdef",
      // get this from antti@meetin.gs
      // is most probably a case insensitive hex string
      
      "api_secret" : "1234567890abcdef",
      // get this from antti@meetin.gs
      // is most probably a case insensitive hex string
      
      "full_sync_start_hour" : 23,
      // daily full sync starts after 23:00
      
      "full_sync_skip_hour" : 4,
      // if daily full sync has not started by 04:00, skip full sync for the day
      // useful for checking wether process should do a full sync right at startup
      // specify the same as start_hour to always sync on startup
    }
  
Logic on startup:

    if (
         ( start_hour > skip_hour && current_hour < start_hour && current_hour > skip_hour ) ||
         ( start_hour < skip_hour && ( current_hour < start_hour || current_hour > skip_hour )
       ) {
      skip_full_sync_until_next_start_hour();
    }

TODO: insctructions for registering as a service which is spun up when machine starts and restarted if it crashes

# Maintenance

What maintenance needs to be done? How to capture logs for debugging?

Now the service writes log in Windows events log. I don't know if there is any idea to send emails (to somebody) in case of problems. Basically Meetin.gs-side of service will know there is something wrong if a) there is no request for email list at 23 o'clock or b) there is no new calendar info incoming a bit after 23 o'clock.
