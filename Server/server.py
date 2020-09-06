#!/usr/bin/env python3

import os

from flask import Flask, request, jsonify

app = Flask(__name__)

LEVELS_DIR = './levels'

levels = []
levels_jsons = {}

last_update = 202003201444

ERR_INVALID_JSON = 'ERR_INVALID_JSON'
ERR_INVALID_JSON_STRUCTURE = 'ERR_INVALID_JSON_STRUCTURE'

def form_error(code):
	print('error:', code)
	return jsonify({
		'error': code,
		'success': False,
	})

@app.route('/summary')
def summary():
	return jsonify({
		'lastUpdate': last_update,
		'levels': levels,
	})

@app.route('/l/<int:id>/json')
def level_json(id):
	return levels_jsons[id]

@app.route('/u/<string:author>/upload', methods=['GET', 'POST'])
def upload_level(author):
	print(request.data)
	if not request.is_json:
		return form_error(ERR_INVALID_JSON)

	level = request.json
	if 'name' not in level:
		return form_error(ERR_INVALID_JSON_STRUCTURE)
	info = {
		'id': len(level),
		'name': level['name'],
		'author': author,
	}
	levels.append(info)
	levels_jsons[info['id']] = request.data
	print('added level with id', info['id'])
	return jsonify({ 'success': True, 'levelId': info['id'] })

def load():
	global idcnt
	for path in os.listdir(LEVELS_DIR):
		if path.endswith('.json'):
			name, author, _ = path.split('.')
			idx = len(levels)
			levels.append({
				'id': idx,
				'name': name,
				'author': author,
			})
			with open(LEVELS_DIR + '/' + path) as f:
				levels_jsons[idx] = f.read()
	print('loaded', len(levels), 'levels')

load()
