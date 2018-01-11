# Aum Security Permission Model

## Overview

It is not a secret that application security is not a simple topic. In this post I will concentrate on the Authorization API side of it - something that is usually overlooked at the original design phases of most applications, then added later.

**Aum framework uses permission-based security** where named permission sets are called "roles", in other words - **Aum uses role-based security at the superficial level**, that goes deeper down to permission level. This approach is much more granular than typical frameworks like ASP.MVC because we go down to permission level when we run methods, show pages, glue contracts etc.


## What are permissions?

Permission are pieces of security-addressable functionality. We support two kinds of permissions: typed and ad-hoc permissions. Typed permissions are specified in code, and by definition, their namespace and class name establish a presence in security data space of authorization store. Ad-hoc permissions are not typed and must specify their string name and path.
```cs
[Glued]
[AuthenticationSupport]
public interface ITestingContract
{
  [AdHocPermission("/Testing/CategoryA", "Echo", AccessLevel.VIEW_CHANGE)] //adhoc permission
  string Echo(string text);
 
  [OneWay] 
  [NotificationsPermission] //typed permission
  void Notify(string text);
 
  object ObjectWork(object dummy);
}
```

In most existing systems, permissions are specified as string literals (ASP.MVC) - similar to AdHocPermission in Aum. The advantage of this approach is simplicity for applications that do not need many security-addressable/guarded functions. However, when applications start growing big, it is not fun to keep repeating the same permission name in UI, Web server, App server and maybe 10 other places. Typed permissions solve this by providing a type-safe check at compile time - if you mistype you get an error.

Another great benefit of typed permissions is their inherent imperative nature.


## Imperative Typed Permissions

Imperative permissions are typed permissions that specify permission/usecase-specific security conditions in their constructor and may override Check method. Think about it this way - a permission is not a boolean flag anymore, it has a logic of its own. This is a form of refactoring - instead of writing IF statements in every place where we check the permission, we rely on permission to do this work. Ultimately, the act of authorization obtains this boolean flag - PASS/FAIL, but how does it obtain it? Having a boolean flag in your permission store is sometimes not enough as you need to write more IF statements that check more things.

Lets take a **concrete example**. Suppose we are building a point-of-sale (POS) application for a distribution club (like COSTCO) where every customer is known at the point of checkout. Suppose, we have some club member roles/levels like: SILVER, GOLD, PLATINUM - where every role defines a set of say 100 permissions. "AlcoholCheckout" is one of them. This permission is granted for GOLD level roles and up. What does this mean? This means that an under-aged customer may buy alcoholic beverages if he/she has earned enough membership credits.

How is this problem addressed? Checking just for permission grant is not enough here, as the final authorization decision is based on customer's age. In Aum we would do an imperative typed permission check like so:
```cs
public class CheckoutPerformer
{
  [AlcoholCheckoutPermission] //notice: no extra code here
  void CheckoutAlcohol(CartItem item);
  ......
}
 
 
public class AlcoholCheckoutPermission : TypedPermission
{ 
  public override bool Check()
  {
    return base.Check() && Session.User.Age > 21;
  } 
}
```

What we could also do here, instead of checking against a constant (21), we could have looked up legal alcohol sale age limit for the user, depending on his/her locality. This approach allows us to use the same simple authorization schemes in the most complicated scenarios. Having moved the Check logic into a typed permission class we are no-longer limited by boolean checks. For example - we can now inject minimum desired AccessLevel at the point of application which is guarded.
```cs
//In this example we supply security assertion in constructor call
//Only registered voters can donate if they have enough access level
[PageTemplatePermission(AccessLevel.VIEW_CHANGE, UserKind.RegisteredVoter)]
public class MayorReElectionDonationPage : WebTemplate
{
  ......
}
 
//Same permission as above. Check is not user-dependent
[PageTemplatePermission(AccessLevel.VIEW)]
public class RallyInvitationPage : WebTemplate
{
  ......
    
  //This will not be called if authorization assertion of linked page fails
  [LinkPagePermission(typeof(MayorReElectionDonationPage)] 
  private donationSection()
  {
    ...... //emit URL link to MayorReElectionDonationPage
  }  
}
```

---
Dmitriy Khmaladze  
July 22, 2013