using UnityEngine;
using System.Collections;
using strange.extensions.context.impl;
using strange.extensions.context.api;

public class RootContext : MVCSContext, IRootContext
{

	public RootContext(MonoBehaviour view) : base(view)
    {

    }

    public RootContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
    {

    }

    protected override void mapBindings()
    {
        base.mapBindings();

        GameObject managers = GameObject.Find("Managers");
        GameObject resources = GameObject.Find("Resources");
        GameObject ui = GameObject.Find("UI");

        injectionBinder.Bind<IRootContext>().ToValue(this).ToSingleton();

        EventManager eventManager = managers.GetComponent<EventManager>();
        injectionBinder.Bind<IEventManager>().ToValue(eventManager).ToSingleton();

        GameManager gameManager = managers.GetComponent<GameManager>();
        injectionBinder.Bind<IGameManager>().ToValue(gameManager).ToSingleton();

        BoardManager boardManager = managers.GetComponent<BoardManager>();
        injectionBinder.Bind<IBoardManager>().ToValue(boardManager).ToSingleton();

        UnitMaterialManager unitMaterialManager = resources.GetComponent<UnitMaterialManager>();
        injectionBinder.Bind<IUnitMaterialManager>().ToValue(unitMaterialManager).ToSingleton();

        UnitImageManager unitImageManager = resources.GetComponent<UnitImageManager>();
        injectionBinder.Bind<IUnitImageManager>().ToValue(unitImageManager).ToSingleton();

        UnitManager unitManager = managers.GetComponent<UnitManager>();
        injectionBinder.Bind<IUnitManager>().ToValue(unitManager).ToSingleton();

        SelectionManager selectionManager = managers.GetComponent<SelectionManager>();
        injectionBinder.Bind<ISelectionManager>().ToValue(selectionManager).ToSingleton();

        CoverManager coverManager = managers.GetComponent<CoverManager>();
        injectionBinder.Bind<ICoverManager>().ToValue(coverManager).ToSingleton();

        UnitMenuManager unitMenuManager = ui.GetComponent<UnitMenuManager>();
        Inject(unitMenuManager);

        UnitTutorialMenu unitTutorialMenu = ui.GetComponent<UnitTutorialMenu>();
        Inject(unitTutorialMenu);

        ContinueScript continueScript = ui.GetComponent<ContinueScript>();
        Inject(continueScript);
    }

    public void Inject(Object o)
    {
        injectionBinder.injector.Inject(o);
    }
}
