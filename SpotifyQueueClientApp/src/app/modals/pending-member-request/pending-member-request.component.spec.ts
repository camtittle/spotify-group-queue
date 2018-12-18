import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PendingMemberRequestComponent } from './pending-member-request.component';

describe('PendingMemberRequestComponent', () => {
  let component: PendingMemberRequestComponent;
  let fixture: ComponentFixture<PendingMemberRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PendingMemberRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PendingMemberRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
