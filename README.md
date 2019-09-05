# StateAtDisk

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 8.3.0.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Express server

To test POST from registration form on Angular front end, run command 'node server' in the root directory which will run a server on port 3000.

## Registration Microservice POST to external site

To test POST to API endpoint https://api.skidata.com/82/v1/user, run command 'dotnet run' in root directory which will run on localhost:5001. Complete the registration form and submit.
Expected result: registration form displays welcome message with no errors.

# Code Retrospective

I had never built an app using Angular before this project, so I had to learn a lot about the framework's structure and syntax quickly. The most difficult part of this was that so many updated versions of Angular have been released in so short a time that many of the references I tried to use had outdated information. This led to challenges when attempting to import the most up-to-date versions of form and http helper modules, for example. However, most of the concepts in Angular were easy for me to grasp, like interpolation, two-way data binding, and dependency injection.

In Angular, the biggest challenge I had was dealing with observables and error handling with the form submit. I had the syntax wrong at first because I was using references to the pre-Angular 6 Http module, and was using .catch instead of .pipe. My repeated, futile attempts to get it to work reminded me of the scene in the original Jurassic Park where Samuel L Jackson's character is trying to regain access to the system (you can watch the clip at https://youtu.be/RfiQYRn7fBg). In the movie, after several attempts he is locked out of the system and a gif of Dennis Nedry appears on the screen, saying in an endless loop, "ah ah ah! You didn't say the magic word!" When I finally realized my error and (much to my relief!) got it to work, I decided to have a little programming fun with my form design and incorporate that same Dennis Nedry gif with my form validation. I took it perhaps a step too far when I decided to also incorporate the sound bite from the movie. If your sound is on and you have any errors on the form, the sound will play on an endless loop until the errors are resolved. This is admittedly a terrible UX decision, but for a personal project is pure entertainment. My only regret is that I couldn't get the "onblur" attribute to work for the FormBuilder class, so the form will show validation errors while typing in the confirm password field until the entry matches the password field. But at least that means everyone gets to endure/enjoy the taunt of "ah ah ah! You didn't say the magic word!" for at least a few seconds.

To test out the Registration Service, which handles POSTing the form data, I ran a Node Express server with settings in server/server.js. This was pretty straightforward and I was pleased to get a success response from the express server when testing the form right away.

After that testing was successful, I added in the .NET Core backend to the project. I have built many microservices with .NET Core before, but never with an Angular front-end, so it was a bit of a challenge to figure out how to incorporate the Angular app and have the two communicate seamlessly. There is some Cors Middleware included in the project that turned out to be unnecessary. I learned .NET Core can be picky about the load order for options in the Configure method in Startup.cs: the CorsPolicy has to be before UseMvc or the controller routing won't work correctly, or something along those lines. I also learned that a POST request will be completely ignored if it’s from the same origin, because CORS headers are only included if the request is cross-origin. It makes perfect sense, but I had never thought about it before, and as a result I couldn’t get the web form POST to hit any of the breakpoints in my Registration.cs file for a time.

After I figured out the CORS issue and ran dotnet build, I was able to test out the form by creating a mock server endpoint using Postman. It was very exciting to see it all work together, from the Angular input form POSTing to the .NET Core backend microservice and onto an external endpoint successfully.

My next challenge was to create a docker image and containerize the microservice so it could be deployed universally. Since Docker requires Hyper-V to run on Windows, and Hyper-V is only included in Windows 10 Pro, SKIDATA generously lent me access to an AWS VM running Windows 10 Pro so I could employ use of Docker CLI. I ran into an issue after the initial install of Docker where after the VM rebooted, one of the Hyper-V components would not initialize. Having experienced a similar issue before while using Docker on an Azure VM, I knew of a few places I could check for a fix. The first fix I tried involved going to the override settings located in Windows Security > App & Browser Control > Exploit Protection Settings > Program Settings, then finding the VMware file, and unchecking “Override System Settings” for Control Flow Guard. After restarting the VM, the component appeared to work for a time and I was able to run Docker commands in the CLI.

The next time I accessed the VM, the same Hyper-V component was not working. Unfortunately, the previous fix seemed to have no effect. The event logs indicated it was an issue with the driver, but when I tried updating the driver for that component, it was already using the most up-to-date version. As a result, I was not able to containerize the registration microservice. Because my previous experience with microservices has involved .NET Core services communicating only with each other and with cshtml front-ends, building images and putting them together with a Docker compose file was a fairly simple process. I am interested to know if containerization would work differently when employing a front-end using a different framework and making a POST call to an external site.

When testing the final product (without containerization), I had some trouble hitting the correct API endpoint with a valid API key. The API documentation in the Loyalty Portal indicated the correct endpoint would be https://apistage.skidataus.com/user/82/v1/user/. GET requests to this point returned error "Invalid APIKey." The Swagger spec file indicates a GET request also requires a bearer token, so that may be the issue there. POST requests to the same endpoint do not require a bearer auth token per the Swagger spec file. Test POST requests did not return Invalid APIKey, but initially returned error 415 "Unsupported Media Type." This made me realize my requests were being said in plain text format instead of JSON. I corrected the issue in the code by retaining the initial JSON object sent by the registration service and posting as JSON using PostAsJsonAsync, and confirmed the headers were showing Application/Json format. I then started receiving error BadRequest.

At this point I realized my error handling between the back and front ends was not airtight. Some errors returned to the front-end were still showing a success message on the Angular side. I had to make changes to the error handling on both sides to return an accurate response to the user.

For the BadRequest error, from the documentation on the Loyalty developer portal, it looks like it may be a formatting issue. The Swagger spec file and User Registration documentation may indicate the following format is required:
{"Username": "test",
"RegistrationChannel": "test-skidata-app",
"UserProfileProperties": [
{"ProfilePropertyName": "Email",
"Value": "test@test.com"},
{"ProfilePropertyName": "Password",
"Value": "test"}
]

I am now working on correcting the format for further testing.
