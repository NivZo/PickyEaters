using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract partial class PagedScreen<TContent> : Node
    where TContent : Node
{
    public int CurrentPage = 0;
    private int _pageCount = 0;
    private Node _contents;
    private RichTextLabel _pageCountLabel;

    public override void _Ready()
    {
        base._Ready();
        _contents = GetNode<Node>("PagedScreen/Contents");
        _pageCountLabel = GetNode<RichTextLabel>("PagedScreen/GUILayer/PageCount");
        _pageCount = GetPageCount();

        GetNode<PagedScreenNextPage>("PagedScreen/GUILayer/NextPage").Setup(
            () => CurrentPage < _pageCount-1,
            NextPage);
        GetNode<PagedScreenPrevPage>("PagedScreen/GUILayer/PrevPage").Setup(
            () => CurrentPage > 0,
            PrevPage);

        Setup();
    }

    protected abstract List<TContent> CreateContents(int pageId);

    protected abstract int GetPageCount();
    
    public void Setup()
    {
        _pageCountLabel.Text = TextUtils.WaveString($"{CurrentPage+1}/{_pageCount}");
        _contents.GetChildren().ToList().ForEach(child => child.QueueFree());
        var currContent = CreateContents(CurrentPage);
        currContent.ForEach(child => _contents.AddChild(child));
    }

    public void NextPage()
    {
        if (CurrentPage < _pageCount-1)
        {
            CurrentPage += 1;
            Setup();
        }
    }

    public void PrevPage()
    {
        if (CurrentPage > 0)
        {
            CurrentPage -= 1;
            Setup();
        }
    }
}
