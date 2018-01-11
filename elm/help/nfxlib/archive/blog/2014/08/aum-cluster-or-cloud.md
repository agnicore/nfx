# Aum Cluster or Cloud?

## Disambiguation and some FAQ
What is a cloud-system? How does it relate to clustering? What about server virtualization?

There are many definitions, each bringing their own little flavor, leaving some confusion aftertaste. 
If you inoculate yourself against correctness angst, trendy words and "term juggling", become nonchalant and just look at facts 
then you would realize that there is an entanglement of a completely orthogonal ontologies.

### Virtual Servers
At first let's get virtualization out of the game. One can build a cloud/cluster/large system without any virtualization whatsoever. 
The confusion comes from the fact that many service providers these days offer "clouds" as a dynamic sets of "virtual"(not real) logical computers 
that customers can create/delete/start/stop. Those computers are not real hardware, there are one way or another emulated as-if being real. 
Of course those "logical" computers run on some physical machines, but this behavior is transparent. The point is - **"virtualization" neither does nor does not automatically make your application for-"cloud"**. 
Your app must be created for cloud regardless of virtualization.

Virtualization is good for cloud apps because it allows to dynamically increase/decrease your server usage by adding/removing boxes as you need them, 
thus you can better utilize your hardware, BUT **if your app is not created to dynamically (at runtime) adjust its participating nodes/servers then virtualization benefits get diminished**.

### What are "Clouds"?
A cloud is just an abstraction of "somewhere on the internet". A cloud-system is a system that runs on some servers in some data centers, 
NOT on your laptop/tablet/phone, although local devices of course interface with clouds, they do not store your data - less headache! 
Contrary to what many believe, clouds do not have to have public access (such as Facebook or Twitter), indeed many corporations built their own internal clouds 
that only internal resources/devices/employees can connect to ( i.e. via VPN).

### Server Clusters?
As demands grow systems end up employing many servers to do some job. i.e. serving database requests or building web pages. 
**A cluster is a set of machines that appear as a single logical system that performs some specific task. 
The machines are usually tightly connected in a data-center and may even span multiple geographical data centers.**


### Am I in the cloud now?
Simple. If all of your personal PCs/tablets/gadgets get fried today, will you lose your data/software in question? 
If yes, then you are not in the modern cloud. Modern cloud systems give you this benefit - just remember your ID and password, and you can continue 
where you left off from any machine/point in the world. This rule is for general apps that are usually web-based. Of course there are special kinds of apps (like 3d games) 
that would require to re-install something on the new computer, but still you would recover all of you "state" where you left off before all of your devices got lost.

### Do I need to use clusters to be in the cloud?
Most likely your cloud system does consist of some form of cluster software/hardware. But the answer is NO. 
One may create a cloud service out of many disjoint computers (that someone else may call a "cluster").

### Do I need to use virtual servers to built clouds?
Absolutely not. Any cloud service can be created without a single virtual computer.

### What are "cloud-apps"?
These are applications engineered to run in the cloud. Usually these are systems that know how to deal with myriads of problems that do not exist in "regular"/local apps. 
For example, in cloud clusters there are many servers to deal with, how does the app get configuration/connect strings to other members of the cloud? 
There are 100s of questions that cloud apps need to address that local (or small client/server apps) don't care about.

### Can I AUTO-convert (without spending time) my existing client/server DB app into a "cloud-app"?
If you still expect to have 5-10 active user then yes, no need to convert. Just host your current client-server app on something like Amazon, 
and nothing needs to be changed (except for some config files). On the other hand, it is not going to be what guys like Google, Facebook, Twitter call "cloud app". 
There is no way to auto-convert your client-server application into a scale-able web service that services 1,000,000 customers a day. 
You need absolutely different architecture for that.

**To Summarize**: Cloud systems are in the cloud (literally somewhere else). Clustering is just a way of sticking many computers together (either physically or logically). 
Cloud services are usually comprised of software and hardware clusters of all sorts. They run applications that were engineered with all crazy cloud system nuances in mind 
(and cost a lot of $$$:(). And finally, virtualization is not a necessary (although convenient for some) requirement to be in the cloud.

## Aum Cluster
"Aum Cluster" is a software library/framework for creation of massive general-purpose computer clusters that may be used to create public/private cloud-based applications. 
The "general-purpose clusters" means - many computers that perform app-dependent tasks, for example, unlike Oracle Database Cluster, 
which is a strictly-speaking just a name for Oracle's database product. Aum Cluster is a library, which means - you build what you want with it, 
be it particle physics simulation or online e-commerce site.

The purpose of Aum Cluster is to address 100s of very complex software problems that arise in distributed systems, 
so its users may concentrate on business-specific tasks. For example: things like configuration of 1,000,000s of servers, discovery, 
peer name resolution, unique ID gen, replicated data stores, process management and remote control, security is all factored in.

What sets Aum Cluster aside from many "cloud systems" is the Unistack approach. 
Unistack is a unified software library that gets deployed to all participating servers thus reducing the complexity 10-fold. 
I have blogged about it [before](/archive/blog/2014/07/unistack-form-of-intellectual-property-compression.html "UNISTACK - a form of Intellectual Property Compression").

Aum Cluster can run on either virtual or physical servers. Virtualization has no real significance when you write your app.

**To Summarize**: Aum Cluster framework allows you to properly architect and build huge systems (with millions of nodes) 
taking care of 100s of complex issues that exist in any distributed system. 
It is like Google/Facebook/Twitter internal mechanisms made available to any application in a general way.

---
Dmitriy Khmaladze  
August 17, 2014