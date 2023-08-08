﻿import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';

export class NavMenu extends Component {
  displayName = NavMenu.name

  render() {
      return (
          <Navbar inverse fixedTop fluid collapseOnSelect>
              <Navbar.Header>
                  <Navbar.Brand>
                      <Link to={'/'}>Avery 16262 Labels</Link>
                  </Navbar.Brand>
                  <Navbar.Toggle />
              </Navbar.Header>
              <Navbar.Collapse>
                  <Nav>
                      <LinkContainer to={'/'} exact>
                          <NavItem>
                              <Glyphicon glyph='home' /> Home
                          </NavItem>
                      </LinkContainer>

                      <LinkContainer to={'/Brewcrafters'}>
                          <NavItem>
                              Brewcrafters
                          </NavItem>
                      </LinkContainer>


                      <LinkContainer to={'/Dominion'}>
                          <NavItem>
                              Dominion
                          </NavItem>
                      </LinkContainer>

                      <LinkContainer to={'/AeonsEnd'}>
                          <NavItem>
                              Aeon's End
                          </NavItem>
                      </LinkContainer>

                      <LinkContainer to={'/Legendary'}>
                          <NavItem>
                              Legendary
                          </NavItem>
                      </LinkContainer>

                      <LinkContainer to={'/SpiritIsland'}>
                          <NavItem>
                              Spirit Island
                          </NavItem>
                      </LinkContainer>

                  </Nav>
              </Navbar.Collapse>
          </Navbar>
      );
  }
}
